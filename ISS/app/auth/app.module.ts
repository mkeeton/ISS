import { NgModule } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { Http, HttpModule } from "@angular/http";
import { BrowserModule } from "@angular/platform-browser";
import { UrlSerializer } from '@angular/router';
import { AppComponent } from "./Components/app.component";
import { AuthComponent } from "./Components/auth.component";
import { ForgottenPasswordComponent } from "./Components/forgottenPassword.component";
import { LoginComponent } from "./Components/login.component";
import { PasswordStrengthBar } from "./Components/passwordStrength.component";
import { ResetPasswordComponent } from "./Components/resetPassword.component";
import { AppRoutingModule } from "./Modules/app-routing.module";
import { LowerCaseUrlSerializer } from "./Modules/urlserializer.module";

@NgModule({
    bootstrap: [AppComponent],
    declarations: [AppComponent, AuthComponent, ForgottenPasswordComponent, LoginComponent, PasswordStrengthBar, ResetPasswordComponent],
    imports: [AppRoutingModule, BrowserModule, FormsModule, HttpModule, ReactiveFormsModule],
    providers: [
        {
            provide: "AuthApiBaseUrl",
            useValue: "http://integratedsoftwaresolutions.co.uk",
        },
        {
            provide: "UserApiBaseUrl",
            useValue: "http://integratedsoftwaresolutions.co.uk",
        },
        {
            provide: UrlSerializer,
            useClass: LowerCaseUrlSerializer,
        },

    ],
})

export class AppModule { }