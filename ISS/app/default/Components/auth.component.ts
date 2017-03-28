import { Component, Inject, OnInit } from "@angular/core";
import { AuthenticationService } from "../Services/authentication.service";
import { StoredSettingService } from "../Services/storedSettings.service";
declare var easyXDM: any;

@Component({
    selector: "auth",
    templateUrl: "auth.component.html",
})

export class AuthComponent implements OnInit {

    constructor(private authService: AuthenticationService,
        private settingService: StoredSettingService) {
    }

    public ngOnInit() {
        let socket = new easyXDM.Socket({
            onMessage: (message, origin) => {
                if (message === "Logout") {
                    this.authService.logoutAsync()
                        .subscribe((res: boolean) => {
                            socket.postMessage("");
                        },
                        (err) => {
                            socket.postMessage("");
                        });
                } else {
                    if (message.indexOf("RemoteLogin:") > -1) {
                        this.authService.remoteLoginAsync(message.replace("RemoteLogin:", ""))
                            .subscribe((res: boolean) => {
                                socket.postMessage(res);
                            });
                    } else {
                        this.authService.authenticateAsync(message)
                            .subscribe((res: string) => {
                                socket.postMessage(res);
                            },
                            (err) => {
                                socket.postMessage("");
                            });
                    }
                }
            },
        });
    }

}