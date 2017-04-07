import { Component, Inject, OnInit } from "@angular/core";
import { AuthenticationService } from "../Services/authentication.service";
import { EasyXDMService } from "../Services/easyXDM.service";
import { StoredSettingService } from "../Services/storedSettings.service";
declare var easyXDM: any;

import "rxjs/add/operator/switchMap";

@Component({
    selector: "auth",
    templateUrl: "auth.component.html",
})

export class AuthComponent implements OnInit {

    constructor(private authService: AuthenticationService,
        private easyXDMService: EasyXDMService,
        private settingService: StoredSettingService) {
    }

    public ngOnInit() {
        let easyXDMAuth = this.easyXDMService.getXDMInstance("Authentication");
        let socket = new easyXDMAuth.Socket({
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
                        this.authService.authenticateAsync(message)
                            .subscribe((res: string) => {
                                socket.postMessage(res);
                            },
                            (err) => {
                                socket.postMessage("");
                            });
                }
            },
        });
    }

}