import { Component, OnInit } from "@angular/core";

import { AuthComponent } from "./auth.component";
import { ForgottenPasswordComponent } from "./forgottenPassword.component";
import { LoginPageComponent } from "./login.component";
import { ResetPasswordComponent } from "./resetPassword.component";

import { APIService } from "../Services/api.service";
import { AuthenticatedService } from "../Services/authenticated.service";
import { AuthenticationService } from "../Services/authentication.service";
import { EasyXDMService } from "../Services/easyXDM.service";
import { StoredSettingService } from "../Services/storedSettings.service";
import { UserService } from "../Services/user.service";

@Component({
    providers: [APIService,
        AuthenticatedService,
        AuthenticationService,
        EasyXDMService,
        StoredSettingService,
        UserService],
    selector: "authentication-app",
    styleUrls: ["../Styles/app.component.css"],
    templateUrl: "app.component.html",
})

export class AppComponent {
}