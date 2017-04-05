import { Inject, Injectable } from "@angular/core";
import { Observable } from "rxjs/Observable";
declare var easyXDM: any;

@Injectable()
export class EasyXDMService {

    public xdmInstance(namespace: string): any {// , elementId: any): any {

        // let element = document.getElementById(elementId)
        // let easyXDMElement,
        //    scripts = document.getElementsByTagName("script"),
        //    scriptIdx;
        // let isEasyXDMPresent = false;
        // console.log(scripts);
        // console.log("Looking for easyXDM");
        // for (scriptIdx = 0; scriptIdx < scripts.length; scriptIdx = scriptIdx + 1) {
        //    if (scripts[scriptIdx].src === "http://localhost:61621/scripts/easyXDM/easyXDM.debug.js") {
        //        isEasyXDMPresent = true;
        //        console.log("Found easyXDM");
        //    }
        // }
        // if (isEasyXDMPresent === false || easyXDM === undefined) {
        //    easyXDMElement = document.createElement("script");
        //    easyXDMElement.type = "text/javascript";
        //    easyXDMElement.src = "http://localhost:61621/scripts/easyXDM/easyXDM.debug.js";
        //    element.parentNode.insertBefore(easyXDMElement, _element);
        //    easyXDMElement.onload = function () {
        //        return easyXDM.noConflict(namespace);
        //    };
        // }
        // else {
        return new easyXDM(namespace);
        // }

        // return easyXDM.noConflict(_namespace);
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