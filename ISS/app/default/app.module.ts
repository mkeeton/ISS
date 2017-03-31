import { NgModule } from "@angular/core";
import { AngularISSauthenticationModule } from "angular-iss-authentication";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { Http, HttpModule } from "@angular/http";
import { BrowserModule } from "@angular/platform-browser";
import { AppComponent } from "./Components/app.component";
import { AuthComponent } from "./Components/auth.component";
import { ForgottenPasswordComponent } from "./Components/forgottenPassword.component";
import { LoginPageComponent } from "./Components/login.component";
import { PasswordStrengthBar } from "./Components/passwordStrength.component";
import { ResetPasswordComponent } from "./Components/resetPassword.component";
import { AppRoutingModule } from "./Modules/app-routing.module";

@NgModule({
    bootstrap: [AppComponent],
    declarations: [AppComponent, AuthComponent, ForgottenPasswordComponent, LoginPageComponent, PasswordStrengthBar, ResetPasswordComponent],
    imports: [AppRoutingModule, AngularISSauthenticationModule.forRoot(), BrowserModule, FormsModule, HttpModule, ReactiveFormsModule],
    providers: [
        {
            provide: "AuthApiBaseUrl",
            useValue: "http://integratedsoftwaresolutions.co.uk",
        },
        {
            provide: "UserApiBaseUrl",
            useValue: "http://integratedsoftwaresolutions.co.uk",
        },
    ],
})

export class AppModule { }