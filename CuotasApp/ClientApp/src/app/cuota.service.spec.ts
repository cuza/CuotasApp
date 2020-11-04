import { TestBed, inject } from '@angular/core/testing';

import { CuotaService } from './cuota.service';

describe('CuotaService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [CuotaService]
    });
  });

  it('should be created', inject([CuotaService], (service: CuotaService) => {
    expect(service).toBeTruthy();
  }));
});
