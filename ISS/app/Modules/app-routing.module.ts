import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";

import { AuthComponent } from "../Components/auth.component";
import { ForgottenPasswordComponent } from "../Components/forgottenPassword.component";
import { LoginComponent } from "../Components/login.component";
import { ResetPasswordComponent } from "../Components/resetPassword.component";

const routes: Routes = [
    { path: "", redirectTo: "/login", pathMatch: "full" },
    { path: "login", component: LoginComponent },
    { path: "auth", component: AuthComponent },
    { path: "Auth", component: AuthComponent },
    { path: "ForgottenPassword", component: ForgottenPasswordComponent },
    { path: "ResetPassword", component: ResetPasswordComponent },
];

@NgModule({
    exports: [RouterModule],
    imports: [RouterModule.forRoot(routes)],
})
export class AppRoutingModule { }