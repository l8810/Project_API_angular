import { TestBed } from '@angular/core/testing';

import { DonorListService } from './donor-list.service';

describe('DonorListService', () => {
  let service: DonorListService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(DonorListService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
