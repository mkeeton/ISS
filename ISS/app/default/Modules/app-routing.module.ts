import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";

import { DefaultComponent } from "../Components/default.component";

const routes: Routes = [
    { path: "default/:id", component: DefaultComponent },
    { path: ":id", redirectTo: "/default/:id", pathMatch: "full" },
];

@NgModule({
    exports: [RouterModule],
    imports: [RouterModule.forRoot(routes)],
})
export class AppRoutingModule { }