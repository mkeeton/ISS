import { Injectable } from "@angular/core";
import { Headers, Http, Response } from "@angular/http";
import { Observable } from "rxjs/Observable";
import { APIResponse, APIService } from "./api.service";
import { AuthenticationService } from "./authentication.service";
import { StoredSettingService } from "./storedSettings.service";

@Injectable()
export class AuthenticatedService {

    constructor(private http: Http,
        private apiService: APIService,
        private authService: AuthenticationService,
        private settingService: StoredSettingService) { }

    public getAsync(serviceBaseURL: string, requestUri: string): Observable<APIResponse> {
        let remoteData: Observable<APIResponse>;
        remoteData = new Observable((observer) => {
            let authToken: string = this.settingService.getSetting("authToken_" + serviceBaseURL);
            let responseCode: number = 200;
            let headers = new Headers();
            headers.append("Content-Type", "application/x-www-form-urlencoded");
            if (authToken !== "") {
                headers.append("Authorization", "Bearer " + authToken);
            }
            this.apiService.getAsync(serviceBaseURL, requestUri, headers)
                .subscribe((res: APIResponse) => {
                    if (res.responseCode === 401) {
                        this.authService.authenticateAsync(serviceBaseURL)
                            .subscribe((res2: string) => {
                                if (res2 !== "") {
                                    this.settingService.setSetting("authToken_" + serviceBaseURL, res2);
                                    authToken = res2;
                                    headers = new Headers();
                                    headers.append("Content-Type", "application/x-www-form-urlencoded");
                                    // _authToken= this._settingService.getSetting("authToken_" + serviceBaseURL);
                                    if (authToken !== "") {
                                        headers.append("Authorization", "Bearer " + authToken);
                                    }
                                    this.apiService.getAsync(serviceBaseURL, requestUri, headers)
                                        .subscribe((res3: APIResponse) => {
                                            observer.next(res3);
                                            observer.complete();
                                        },
                                    );

                                } else {
                                    observer.next(res);
                                    observer.complete();
                                }
                            });
                    } else {
                        observer.next(res);
                        observer.complete();
                    }
                }
                , (err) => {
                    observer.next(new APIResponse(err.status, "", err.json()));
                    observer.complete();
                },
            );
        });
        return remoteData;
    }

    public postAsync(serviceBaseURL: string, requestUri: string, requestContent: any): Observable<APIResponse> {
        let remoteData: Observable<APIResponse>;
        remoteData = new Observable((observer) => {
            let authToken: string = this.settingService.getSetting("authToken_" + serviceBaseURL);
            let responseCode: number = 200;
            let headers = new Headers();
            headers.append("Content-Type", "application/x-www-form-urlencoded");
            headers.append("Authorization", "Bearer " + authToken);
            this.apiService.postAsync(serviceBaseURL, requestUri, headers, requestContent)
                .subscribe((res: APIResponse) => {
                    if (res.responseCode === 401) {
                        this.authService.authenticateAsync(serviceBaseURL)
                            .subscribe((res2: string) => {
                                if (res2 !== "") {
                                    this.settingService.setSetting("authToken_" + serviceBaseURL, res2);
                                    authToken = res2;
                                    headers = new Headers();
                                    headers.append("Content-Type", "application/x-www-form-urlencoded");
                                    headers.append("Authorization", "Bearer " + authToken);
                                    this.apiService.postAsync(serviceBaseURL, requestUri, headers, requestContent)
                                        .subscribe((res3: APIResponse) => {
                                            observer.next(res3);
                                            observer.complete();
                                        },
                                    );

                                } else {
                                    observer.next(res);
                                    observer.complete();
                                }
                            });
                    } else {
                        observer.next(res);
                        observer.complete();
                    }
                },
            );
        });
        return remoteData;
    }
}