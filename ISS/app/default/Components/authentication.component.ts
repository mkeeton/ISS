import { ChangeDetectorRef, Component, OnInit, ViewChild } from "@angular/core";
import { Router, ActivatedRoute, Params } from '@angular/router';
import { User, APIService, AuthenticationService, CurrentUserService, EasyXDMService, LoginComponent, StoredSettingService, UserService } from "angular-iss-authentication";
import { DefaultComponent } from "./default.component";

import "rxjs/add/operator/switchMap";

@Component({
    providers: [],
    selector: ".auth-component",
    styleUrls: ["../Styles/authentication.component.css"],
    templateUrl: "./authentication.component.html",
})

export class AuthenticationComponent implements OnInit {

    public currentuser: User;

    public isLoginVisible: boolean;
    public isLoginActive: boolean;

    @ViewChild(LoginComponent) login: LoginComponent;

    constructor(private ref: ChangeDetectorRef,
        private currentUserService: CurrentUserService,
        private router: Router,
        private userService: UserService,
        private authService: AuthenticationService) {
    }

    public ngOnInit() {
        this.currentUserService.dispatcher.subscribe((user) => {
            this.currentuser = user;
            this.ref.detectChanges();
            if (this.currentuser !== null) {
                this.closeLoginClick();
            }
        });
        this.isLoginVisible = false;
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
        this.ref.detectChanges();
        this.login.showLogin(true);
    }

    closeLoginClick() {
        this.isLoginVisible = false;
        this.login.hideLogin();
        this.ref.detectChanges();
    }
}