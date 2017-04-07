import { Inject, Injectable } from "@angular/core";
import { Observable } from "rxjs/Observable";
import { XDMInstance } from "../Classes/xdmInstance";
declare var easyXDM: any;

@Injectable()
export class EasyXDMService {

    private instances: Array<XDMInstance> = new Array<XDMInstance>();

    public getXDMInstance(namespace: string): any {// , elementId: any): any {

        let found: boolean = false;
        for (let i of this.instances) {
            if (i.Namespace === namespace) {
                found = true;
                return i.Instance;
            }
        }
        if (found === false) {
            if (namespace === '') {
                let newInstance: any = easyXDM;
                this.instances.push(new XDMInstance(namespace, newInstance));
                return newInstance;
            }
            else {
                let newInstance: any = easyXDM.noConflict('easyXDM_' + namespace);
                this.instances.push(new XDMInstance(namespace, newInstance));
                return newInstance;
            }
        }

    }

    public Msg(xdmInstance: any, serverURL: string, msg: string): Observable<string[]> {
        let msgData = new Observable((observer) => {
            let msgSocket = new xdmInstance.Socket({
                isHost: true,
                onMessage: (message, origin) => {
                    observer.next(message);
                    observer.complete();
                },
                remote: serverURL,
            });

            msgSocket.postMessage(msg);
        },
        );
        return msgData;
    }

    public Rpc(xdmInstance: any, serverURL: string, procName: string, params: Param[]): Observable<string[]> {
        let rpcData = new Observable((observer) => {
            let rpcRemote = new xdmInstance.Rpc({
                remote: serverURL,
            }, { remote: { postMessage: {} } });

            switch (procName) {
                case "postMessage":
                    rpcRemote.postMessage(params, (result) => {
                        observer.next(result);
                        observer.complete();
                    });
                default:
            }

        },
        );
        return rpcData;
    }

    public IFrame(xdmInstance: any,
        serverURL: string,
        frameContainer: string,
        frameWidth: string,
        frameHeight: string,
    ): Observable<string[]> {
        let remoteFrame = new Observable((observer) => {
            let frameSocket = new xdmInstance.Socket({
                container: document.getElementById(frameContainer),
                onMessage: (message, origin) => {
                    observer.next(message);
                    observer.complete();
                },
                remote: serverURL,
            });
            if (frameWidth !== "") {
                document.getElementById(frameContainer).getElementsByTagName("iframe")[0].style.width = frameWidth;
            }
            if (frameHeight !== "") {
                document.getElementById(frameContainer).getElementsByTagName("iframe")[0].style.height = frameHeight;
            }
        },
        );
        return remoteFrame;
    }

}

class Param {
    public key: string;
    public value: any;
}