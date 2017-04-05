import { Component, OnInit } from "@angular/core";
import { Router, ActivatedRoute, Params } from '@angular/router';
import { User, APIService, AuthenticationService, CurrentUserService, EasyXDMService, StoredSettingService, UserService } from "angular-iss-authentication";
import { DefaultComponent } from "./default.component";

@Component({
    providers: [CurrentUserService],
    selector: "auth-component",
    styleUrls: ["../Styles/authentication.component.css"],
    templateUrl: "./authentication.component.html",
})

export class AuthenticationComponent implements OnInit {

    public currentuser: User;

    public isLoginVisible: boolean;
    public isLoginActive: boolean;

    constructor(private currentUserService: CurrentUserService,
        private router: Router,
        private userService: UserService,
        private authService: AuthenticationService) {
    }

    public ngOnInit() {
        this.currentUserService.dispatcher.subscribe((user) => {
            this.currentuser = user;
            this.currentUserChanged();
        });
        this.isLoginVisible = false;
        this.getCurrentUser();
    }

    public currentUserChanged() {

    }

    public getCurrentUser() {
        this.userService.getCurrentUserAsync()
            .subscribe((user) => {
                this.currentUserService.setCurrentUser(user);
            })
            , ((err) => {
                this.currentUserService.setCurrentUser(null);
            });
    }

    public logoutClick() {
        this.authService.logOut()
            .subscribe((res) => {
                this.currentUserService.setCurrentUser(null);
            });
        return false;
    }

    loginClick() {
        this.isLoginVisible = true;
        this.authService.login("authenticationLogin",true)
            .subscribe(res => {
                this.closeLoginClick();
            });
    }

    closeLoginClick() {
        this.isLoginVisible = false;
        document.getElementById("authenticationLogin").innerHTML = "";
        this.getCurrentUser();
    }
}