import { Component, Inject, OnDestroy, OnInit } from "@angular/core";
import { FormBuilder, Validators } from "@angular/forms";
import { ActivatedRoute, Params, Router } from "@angular/router";

import { User } from "../Classes/Users/user";
import { AuthenticationService } from "../Services/authentication.service";
import { EasyXDMService } from "../Services/easyXDM.service";
import { StoredSettingService } from "../Services/storedSettings.service";
import { UserService } from "../Services/user.service";

import "rxjs/add/operator/switchMap";

@Component({
    selector: "login",
    styleUrls: ["http://fonts.googleapis.com/css?family=Lato:400,700",
        "../Styles/login.component.css", "../Styles/login.component.css"],
    templateUrl: "login.component.html",
})

export class LoginPageComponent implements OnInit, OnDestroy {

    public formOptions = { state: 0 };
    public userList: User[];

    public easyXDMAuth: any;
    public socket: any;

    public displayRegister: string = "";

    public loginError: string = "";
    public registrationError: string = "";

    public myForm = this.fb.group({
        email: ["", Validators.required],
        password: ["", Validators.required],
    });

    public registrationForm = this.fb.group({
        confirmpassword: ["", Validators.required],
        email: ["", Validators.required],
        forename: ["", Validators.required],
        password: ["", Validators.required],
        surname: ["", Validators.required],
    });

    private sub: any;

    constructor(public fb: FormBuilder,
        private authService: AuthenticationService,
        private userService: UserService,
        private settingService: StoredSettingService,
        private easyXDMService: EasyXDMService,
        private route: ActivatedRoute,
        private router: Router) {

        this.sub = route.queryParams.subscribe(
            (queryParam: any) => this.displayRegister = queryParam["wr"],
        );
    }

    public ngOnInit() {
        try {
            this.easyXDMAuth = this.easyXDMService.xdmInstance("Authentication");
            this.socket = new this.easyXDMAuth.Socket({});
        } catch (ex) {

        }
    }

    public ngOnDestroy() {
        this.sub.unsubscribe();
    }

    public onSubmit(value: any): void {
        this.loginUser(value.email, value.password);
    }

    public loginUser(email: any, password: any) {
        this.loginError = "";
        this.authService.loginAsync(email, password)
            .subscribe((res: boolean) => {
                if (res === true) {
                    this.sendResponseMessage("Login Complete");
                } else {
                    this.loginError = "Unknown Error";
                    this.sendResponseMessage("Login Failed");
                }
            }
            , (err) => {
                this.loginError = err;
                this.sendResponseMessage("Login Failed");
            });
    }

    public onRegister(value: any): void {
        this.authService.registerAsync(value.email,
            value.password,
            value.confirmpassword,
            value.forename,
            value.surname)
            .subscribe((res: boolean) => {
                if (res === true) {
                    this.loginUser(value.email, value.password);
                } else {
                    this.sendResponseMessage("Login Failed");
                }
            }
            , (err) => {
                this.registrationError = err;
                this.sendResponseMessage("Login Failed");
            });
    }

    public sendResponseMessage(msg: string): void {
        try {
            if (this.socket !== null) {
                this.socket.postMessage(msg);
            }
        } catch (ex) {
        }
    }
}