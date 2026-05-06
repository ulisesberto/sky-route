import { Routes } from '@angular/router';

import { FlightSearchPage } from './pages/flight-search-page/flight-search-page';

export const routes: Routes = [
  { path: '', component: FlightSearchPage, pathMatch: 'full' },
  { path: '**', redirectTo: '' }
];
