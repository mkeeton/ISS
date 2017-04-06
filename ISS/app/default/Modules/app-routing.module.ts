import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";

import { DefaultComponent } from "../Components/default.component";

const routes: Routes = [
    { path: "default", component: DefaultComponent },
    { path: "", redirectTo: "default", pathMatch: "full" },
];

@NgModule({
    exports: [RouterModule],
    imports: [RouterModule.forRoot(routes)],
})
export class AppRoutingModule { }