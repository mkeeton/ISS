import { Component, OnInit } from "@angular/core";
import { Router, ActivatedRoute, Params } from '@angular/router';

import { StoredSettingService } from "angular-iss-authentication";

declare var easyXDM: any;


@Component({
    selector: "default-page",
    styleUrls: ["../Styles/default.component.css"],
    templateUrl: "./default.component.html",
})

export class DefaultComponent implements OnInit {

    private sub: any;

    constructor(private route: ActivatedRoute) {
    }

    public ngOnInit() {
        //this.sub = this.route.params.subscribe(params => {
        //    console.log("Getting order " + params["id"]);
        //    this.settingService.setSetting("currentOrderId", params["id"]);
        //});
    }
}