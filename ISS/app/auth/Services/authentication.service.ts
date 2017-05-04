import { Inject, Injectable } from "@angular/core";
import { Headers, Http, Response } from "@angular/http";
import { Observable } from "rxjs/Observable";
import { StoredSettingService } from "../Services/storedSettings.service";

@Injectable()
export class AuthenticationService {

    constructor(private http: Http,
        private settingService: StoredSettingService,
        @Inject("AuthApiBaseUrl") private apiBaseUrl: string) { }

    public registerAsync(email: string,
        password: string,
        confirmPassword: string,
        forename: string, surname: string): Observable<boolean> {
        let remoteData: Observable<boolean>;
        remoteData = new Observable((observer) => {
            let headers = new Headers();
            headers.append("Content-Type", "application/json");
            let body = {
                ConfirmPassword: confirmPassword,
                Email: email,
                FirstName: forename,
                LastName: surname,
                Password: password,
            };
            this.http.post(this.apiBaseUrl + "/api/Account/Register", body, { headers: headers })
                .subscribe((res: Response) => {
                    observer.next(true);
                    observer.complete();
                }
                , (err) => {
                    let errorMessage = err.json();
                    observer.error(errorMessage.Message);
                    // observer.next(false);
                    // observer.complete();
                });
        });
        return remoteData;
    }

    public test(): any {

    }

    public authenticateAsync(serviceURL: string): Observable<string> {
        let remoteData: Observable<string>;
        remoteData = new Observable((observer) => {
            let refreshToken: string = this.settingService.getSetting("authRefresh");
            let counter = 0;
            while (refreshToken === "INUSE") {
                if (counter <= 20) {
                    setTimeout(this.test, 200);
                    refreshToken = this.settingService.getSetting("authRefresh")
                    counter += 1;
                } else {
                    refreshToken = "";
                    this.settingService.setSetting("authRefresh", "");
                }
            }
            if (refreshToken !== "") {
                this.settingService.setSetting("authRefresh", "INUSE");
                this.refreshAuthenticationAsync(refreshToken)
                    .subscribe((res: boolean) => {
                        if (res === true) {
                            let authToken: string = this.settingService.getSetting("authToken");
                            let headers = new Headers();
                            headers.append("Content-Type", "application/x-www-form-urlencoded");
                            headers.append("Authorization", "Bearer " + authToken);
                            let body: string = "grant_type=password&client_id=" + serviceURL;
                            this.http.post(this.apiBaseUrl + "/token", body, { headers: headers })
                                .subscribe((res2: Response) => {
                                    let authResponse = res2.json();
                                    observer.next(authResponse.access_token);
                                    observer.complete();
                                }
                                , (err) => {
                                    observer.next("");
                                    observer.complete();
                                });

                        } else {
                            observer.next("");
                            observer.complete();
                        }
                    });

            } else {
                observer.next("");
                observer.complete();
            }
        });
        return remoteData;
    }

    public refreshAuthenticationAsync(refreshToken: string): Observable<boolean> {
        let remoteData: Observable<boolean>;
        remoteData = new Observable((observer) => {
            let headers = new Headers();
            headers.append("Content-Type", "application/x-www-form-urlencoded");
            let body: string = "grant_type=refresh_token&refresh_token=" + refreshToken;
            this.http.post(this.apiBaseUrl + "/token", body, { headers: headers })
                .subscribe((res: Response) => {
                    let authResponse = res.json();

                    this.settingService.setSetting("authToken", authResponse.access_token);
                    this.settingService.setSetting("authRefresh", authResponse.refresh_token);
                    this.settingService.setSetting("authLoginId", authResponse.remote_login_id);
                    observer.next(true);
                    observer.complete();
                }
                , (err) => {
                    this.settingService.setSetting("authToken", "");
                    this.settingService.setSetting("authRefresh", "");
                    observer.next(false);
                    observer.complete();
                });
        });
        return remoteData;
    }

    public loginAsync(username: string, password: string): Observable<boolean> {
        let remoteData: Observable<boolean>;
        remoteData = new Observable((observer) => {
            let headers = new Headers();
            headers.append("Content-Type", "application/x-www-form-urlencoded");
            let body: string = "grant_type=password&UserName=" + username;
            body += "&Email=" + username + "&Password=" + encodeURIComponent(password);

            this.http.post(this.apiBaseUrl + "/token", body, { headers: headers })
                .subscribe((res: Response) => {
                    let authResponse = res.json();

                    this.settingService.setSetting("authToken", authResponse.access_token);
                    this.settingService.setSetting("authRefresh", authResponse.refresh_token);
                    this.settingService.setSetting("authLoginId", authResponse.remote_login_id);
                    observer.next(true);
                    observer.complete();
                }
                , (err) => {
                    this.settingService.setSetting("authToken", "");
                    this.settingService.setSetting("authRefresh", "");
                    let errorMessage = err.json();
                    observer.error(errorMessage.error_description);
                    // observer.next(false);
                    // observer.complete();
                });
        });
        return remoteData;
    }

    public remoteLoginAsync(remoteLoginId: string): Observable<boolean> {
        let remoteData: Observable<boolean>;
        remoteData = new Observable((observer) => {
            let headers = new Headers();
            headers.append("Content-Type", "application/x-www-form-urlencoded");
            let body: string = "grant_type=password&remoteLogin_id=" + remoteLoginId;

            this.http.post(this.apiBaseUrl + "/token", body, { headers: headers })
                .subscribe((res: Response) => {
                    let authResponse = res.json();
                    this.settingService.setSetting("authToken", authResponse.access_token);
                    this.settingService.setSetting("authRefresh", authResponse.refresh_token);
                    this.settingService.setSetting("authLoginId", authResponse.remote_login_id);
                    observer.next(true);
                    observer.complete();
                }
                , (err) => {
                    this.settingService.setSetting("authToken", "");
                    this.settingService.setSetting("authRefresh", "");
                    observer.next(false);
                    observer.complete();
                });
        });
        return remoteData;
    }

    public logoutAsync(): Observable<boolean> {
        let remoteData: Observable<boolean>;
        remoteData = new Observable((observer) => {
            let headers = new Headers();
            headers.append("Content-Type", "application/x-www-form-urlencoded");
            let authToken: string = this.settingService.getSetting("authToken");
            if (authToken !== "") {
                headers.append("Authorization", "Bearer " + authToken);
            }
            this.http.get(this.apiBaseUrl + "/api/Account/Logout", { headers: headers })
                .subscribe((res: Response) => {
                    this.settingService.setSetting("authToken", "");
                    this.settingService.setSetting("authRefresh", "");
                    observer.next(true);
                    observer.complete();
                }
                , (err) => {
                    this.settingService.setSetting("authToken", "");
                    this.settingService.setSetting("authRefresh", "");
                    observer.next(true);
                    observer.complete();
                });
        });
        return remoteData;
    }

    public passwordReminderAsync(email: string): Observable<boolean> {
        let remoteData: Observable<boolean>;
        remoteData = new Observable((observer) => {
            let headers = new Headers();
            headers.append("Content-Type", "application/json");
            let body = {
                Email: email,
            };
            this.http.post(this.apiBaseUrl + "/api/Account/PasswordReminder", body, { headers: headers })
                .subscribe((res: Response) => {
                    observer.next(true);
                    observer.complete();
                }
                , (err) => {
                    observer.next(false);
                    observer.complete();
                });
        });
        return remoteData;
    }

    public confirmResetToken(resetToken: string) {
        let remoteData: Observable<boolean>;
        remoteData = new Observable((observer) => {
            let headers = new Headers();
            headers.append("Content-Type", "application/json");
            let body: string = "\"" + resetToken + "\"";
            this.http.post(this.apiBaseUrl + "/api/Account/ConfirmToken", body, { headers: headers })
                .subscribe((res: Response) => {
                    observer.next(res.json());
                    observer.complete();
                }
                , (err) => {
                    // observer.next(null);
                    // observer.complete();
                    let errorMessage = err.json();
                    observer.error(errorMessage.Message);
                });
        });
        return remoteData;
    }

    public resetPasswordAsync(resetToken: string, password: string, confirmpassword: string) {
        let remoteData: Observable<boolean>;
        remoteData = new Observable((observer) => {
            let headers = new Headers();
            headers.append("Content-Type", "application/json");
            let body = {
                ConfirmPassword: confirmpassword,
                NewPassword: password,
                ResetToken: resetToken,
            };
            this.http.post(this.apiBaseUrl + "/api/Account/PasswordReset", body, { headers: headers })
                .subscribe((res: Response) => {
                    observer.next(true);
                    observer.complete();
                }
                , (err) => {
                    let errorMessage = err.json();
                    observer.error(errorMessage.Message);
                });
        });
        return remoteData;
    }

}