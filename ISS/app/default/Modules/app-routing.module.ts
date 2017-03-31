import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";

import { AuthComponent } from "../Components/auth.component";
import { ForgottenPasswordComponent } from "../Components/forgottenPassword.component";
import { LoginPageComponent } from "../Components/login.component";
import { ResetPasswordComponent } from "../Components/resetPassword.component";

const routes: Routes = [
    { path: "", redirectTo: "/Auth/login", pathMatch: "full" },
    { path: "Auth/Login", component: LoginPageComponent },
    { path: "Auth/login", component: LoginPageComponent },
    { path: "Auth/auth", component: AuthComponent },
    { path: "Auth/Auth", component: AuthComponent },
    { path: "Auth/ForgottenPassword", component: ForgottenPasswordComponent },
    { path: "Auth/ResetPassword", component: ResetPasswordComponent },
];

@NgModule({
    exports: [RouterModule],
    imports: [RouterModule.forRoot(routes)],
})
export class AppRoutingModule { }