import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { BarHederComponent } from './components/bar-heder/bar-heder.component';


@Component({
    selector: 'app-root',
    imports: [RouterOutlet,BarHederComponent],
    templateUrl: './app.component.html',
    styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'client';
}
