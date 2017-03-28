import { Component, Inject, OnDestroy, OnInit } from "@angular/core";
import { FormBuilder, Validators, FormGroup } from "@angular/forms";
import { ActivatedRoute } from "@angular/router";
import "rxjs/add/operator/map";
import "rxjs/add/operator/switchMap";
import "rxjs/add/operator/toPromise";
import { AuthenticationService } from "../Services/authentication.service";
import { PasswordStrengthBar } from './passwordStrength.component';

@Component({
    selector: "resetPassword",
    styleUrls: ["http://fonts.googleapis.com/css?family=Lato:400,700", "../Styles/resetPassword.component.css"],
    templateUrl: "resetPassword.component.html",
})
export class ResetPasswordComponent implements OnInit, OnDestroy {

    public resetOptions = { state: 0 };
    public resetToken: string;

    public fullName: string;
    public errorMessage: string = "";

    public resetForm = this.fb.group({
        confirmpassword: ["", Validators.required],
        password: ["", Validators.required],
    }, { validator: this.areEqual });

    private sub: any;

    constructor(public fb: FormBuilder, private authService: AuthenticationService, private route: ActivatedRoute) {
        this.sub = route.queryParams.subscribe(
            (queryParam: any) => this.resetToken = queryParam["token"],
        );
    }

    public ngOnInit() {
        this.authService.confirmResetToken(this.resetToken)
            .subscribe((res: any) => {
                this.fullName = res.FirstName + " " + res.LastName;
            }
            , (err) => {
                this.errorMessage = err;
                this.resetOptions.state = 2;
            });
    }

    public ngOnDestroy() {
        this.sub.unsubscribe();
    }

    public onSubmit(value: any): void {
        this.authService.resetPasswordAsync(this.resetToken, value.password, value.confirmpassword)
            .subscribe((res: boolean) => {
                this.resetOptions.state = 1;
            }
            , (err) => {
                this.errorMessage = err;
                this.resetOptions.state = 2;
            });
    }

    public areEqual(group: FormGroup) {
        var valid = true;
        let oldVal: string = null;
        for (var name in group.controls) {
            var val = group.controls[name].value
            if ((oldVal != null) && (val != oldVal)) {
                valid = false;
            }
            oldVal = val;
        }

        if (valid) {
            return null;
        }

        return {
            areEqual: true
        };
    }
}