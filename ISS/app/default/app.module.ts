import { NgModule } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from "@angular/forms";
import { Http, HttpModule } from "@angular/http";
import { BrowserModule } from "@angular/platform-browser";
import { UrlSerializer } from '@angular/router';
import { AngularISSauthenticationModule } from "angular-iss-authentication";
import { AppComponent } from "./Components/app.component";
import { AuthenticationComponent } from "./Components/authentication.component";
import { DefaultComponent } from "./Components/default.component";
import { HeaderComponent } from "./Components/header.component";
import { MenuComponent } from "./Components/menu.component";
import { AppRoutingModule } from "./Modules/app-routing.module";
import { LowerCaseUrlSerializer } from "./Modules/urlserializer.module";

@NgModule({
    bootstrap: [AppComponent],
    declarations: [AppComponent, AuthenticationComponent, DefaultComponent, HeaderComponent, MenuComponent],
    imports: [BrowserModule, AngularISSauthenticationModule.forRoot(), AppRoutingModule, FormsModule, HttpModule, ReactiveFormsModule],
    providers: [
        { provide: "AuthApiBaseUrl", useValue: "http://localhost:53303/Auth/auth" },
        { provide: "LoginPageUrl", useValue: "http://localhost:53303/Auth/login" },
        { provide: "UserApiBaseUrl", useValue: "http://localhost:53303/" },
        { provide: "CurrentSiteApiBaseUrl", useValue: "http://localhost:53303/" },
        { provide: UrlSerializer, useClass: LowerCaseUrlSerializer },
    ],
})

export class AppModule { }