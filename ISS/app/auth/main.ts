import { enableProdMode } from "@angular/core";
import { platformBrowser } from "@angular/platform-browser";
import { AppModuleNgFactory } from "../../aot/app/auth/app.module.ngfactory";
enableProdMode();
platformBrowser().bootstrapModuleFactory(AppModuleNgFactory);