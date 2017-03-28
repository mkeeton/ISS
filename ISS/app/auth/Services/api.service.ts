import { Injectable } from "@angular/core";
import { Headers, Http, Response } from "@angular/http";
import { Observable } from "rxjs/Observable";
import { StoredSettingService } from "./storedSettings.service";

@Injectable()
export class APIService {

    constructor(private http: Http, private settingService: StoredSettingService) { }

    public get(serviceBaseURL: string, requestUri: string, headers: Headers): APIResponse {
        let bodge: boolean = true;
        if (bodge === true) {
            this.http.get(serviceBaseURL + requestUri, { headers: headers })
                .subscribe((res: Response) => {
                    let authResponse = res.json();
                    return new APIResponse(res.status, "Success", res.json());
                }
                , (err) => {
                    return new APIResponse(err.status, "", err.json());
                });
        } else {
            return new APIResponse(200, "", "");
        }
    }

    public post(serviceBaseURL: string, requestUri: string, headers: Headers, requestContent: string): APIResponse {
        let bodge: boolean = true;
        if (bodge === true) {
            this.http.post(serviceBaseURL + requestUri, requestContent, { headers: headers })
                .subscribe((res: Response) => {
                    return new APIResponse(res.status, "Success", res.json());
                }
                , (err) => {
                    return new APIResponse(err.status, "", err.json());
                });
        } else {
            return new APIResponse(200, "", "");
        }
    }

    public getAsync(serviceBaseURL: string, requestUri: string, headers: Headers): Observable<APIResponse> {
        let remoteData: Observable<APIResponse>;
        remoteData = new Observable((observer) => {
            this.http.get(serviceBaseURL + requestUri, { headers: headers })
                .subscribe((res: Response) => {
                    observer.next(new APIResponse(res.status, "Success", res.json()));
                    observer.complete();
                }
                , (err) => {
                    observer.next(new APIResponse(err.status, err.statusText, err.statusText));
                    observer.complete();
                });

        },
        );
        return remoteData;
    }

    public postAsync(serviceBaseURL: string,
        requestUri: string,
        headers: Headers,
        requestContent: string): Observable<APIResponse> {
        let remoteData: Observable<APIResponse>;
        remoteData = new Observable((observer) => {
            this.http.post(serviceBaseURL + requestUri, requestContent, { headers: headers })
                .subscribe((res: Response) => {
                    observer.next(new APIResponse(res.status, "Success", res.json()));
                    observer.complete();
                }
                , (err) => {
                    observer.next(new APIResponse(err.status, err.statusText, err.statusText));
                    observer.complete();
                });

        },
        );
        return remoteData;
    }
}

export class APIResponse {
    constructor(public responseCode: number, public responseMessage: string, public responseData: any) { }

}