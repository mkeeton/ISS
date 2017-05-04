import { Component, OnInit } from "@angular/core";
import { User, APIService, AuthenticationService, AuthenticatedService, CurrentUserService, EasyXDMService, StoredSettingService, UserService } from "angular-iss-authentication";

@Component({
    providers: [],
    selector: "my-app",
    styleUrls: ["../Styles/app.component.css"],
    templateUrl: "./app.component.html",
})

export class AppComponent implements OnInit {

    public currentuser: User;

    constructor() {
    }

    public ngOnInit() {

    }

}