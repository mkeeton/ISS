import { Injectable } from "@angular/core";

@Injectable()
export class StoredSettingService {

    public getSetting = (key: string): any => {
        let item: any = localStorage.getItem(key);

        if (item && item !== "undefined") {
            return JSON.parse(localStorage.getItem(key));
        }

        return "";
    }

    public setSetting = (key: string, value: any): void => {
        localStorage.setItem(key, JSON.stringify(value));
    }

    public removeSetting = (key: string): void => {
        localStorage.removeItem(key);
    }
}