export class User {
    constructor(
        public Id: string,
        UserName: string,
        HasPassword: boolean,
        SecurityStamp: string,
        Email: string,
        EmailConfirmed: string,
        FirstName: string,
        LastName: string,
    ) { }
}