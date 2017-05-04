import { Component, OnInit } from "@angular/core";
import { Router, ActivatedRoute, Params } from '@angular/router';

import { StoredSettingService } from "angular-iss-authentication";


@Component({
    selector: "menu-component",
    styleUrls: ["../Styles/menu.component.css"],
    templateUrl: "./menu.component.html",
})

export class MenuComponent implements OnInit {

    private sub: any;

    constructor(private route: ActivatedRoute) {
    }

    public ngOnInit() {

    }
}