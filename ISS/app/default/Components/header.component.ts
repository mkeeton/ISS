import { Component, OnInit } from "@angular/core";
import { Router, ActivatedRoute, Params } from '@angular/router';

import { StoredSettingService } from "angular-iss-authentication";


@Component({
    selector: "header-component",
    styleUrls: ["../Styles/header.component.css"],
    templateUrl: "./header.component.html",
})

export class HeaderComponent implements OnInit {

    private sub: any;

    constructor(private route: ActivatedRoute) {
    }

    public ngOnInit() {

    }
}