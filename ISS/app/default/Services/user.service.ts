import { Inject, Injectable } from "@angular/core";
import { Headers, Http, Response } from "@angular/http";
import { Observable } from "rxjs/Observable";
import { User } from "../Classes/Users/user";
import { AuthenticatedService } from "../Services/authenticated.service";
import { StoredSettingService } from "../Services/storedSettings.service";
import { APIResponse } from "./api.service";

@Injectable()
export class UserService {

    constructor(private authenticatedService: AuthenticatedService,
        private settingService: StoredSettingService,
        @Inject("UserApiBaseUrl") private apiBaseUrl: string,
    ) { }

    public getUsersAsync(): Observable<User[]> {
        let remoteData: Observable<User[]>;
        remoteData = new Observable((observer) => {
            this.authenticatedService.getAsync(this.apiBaseUrl, "api/User/UsersFromRole?roleName=HABC Administrator")
                .subscribe((res: APIResponse) => {
                    if (res.responseCode === 200) {
                        observer.next(<User[]>res.responseData);
                    } else {
                        observer.next(null);
                    }
                    observer.complete();
                });
        });
        return remoteData;
    }
}