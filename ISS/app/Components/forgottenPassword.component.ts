import { Component, Inject } from "@angular/core";
import { FormBuilder, Validators } from "@angular/forms";
import { AuthenticationService } from "../Services/authentication.service";

@Component({
    selector: "forgottenPassword",
    styleUrls: ["http://fonts.googleapis.com/css?family=Lato:400,700", "../Styles/forgottenPassword.component.css"],
    templateUrl: "forgottenPassword.component.html",
})
export class ForgottenPasswordComponent {

    public forgottenOptions = { state: 0 };

    public forgottenForm = this.fb.group({
        email: ["", Validators.required],
    });

    constructor(public fb: FormBuilder, private authService: AuthenticationService) {

    }

    public onSubmit(value: any): void {
        this.authService.passwordReminderAsync(value.email)
            .subscribe((res: boolean) => {
                if (res === true) {
                    this.forgottenOptions.state = 1;
                } else {
                    this.forgottenOptions.state = 2;
                }
            });
    }
}