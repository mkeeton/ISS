import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";

import { AuthComponent } from "../Components/auth.component";
import { ForgottenPasswordComponent } from "../Components/forgottenPassword.component";
import { LoginComponent } from "../Components/login.component";
import { ResetPasswordComponent } from "../Components/resetPassword.component";

const routes: Routes = [
    { path: "", redirectTo: "/Auth/login", pathMatch: "full" },
    { path: "auth/login", component: LoginComponent },
    { path: "auth/auth", component: AuthComponent },
    { path: "auth/forgottenpassword", component: ForgottenPasswordComponent },
    { path: "auth/resetpassword", component: ResetPasswordComponent },
];

@NgModule({
    exports: [RouterModule],
    imports: [RouterModule.forRoot(routes)],
})
export class AppRoutingModule { }