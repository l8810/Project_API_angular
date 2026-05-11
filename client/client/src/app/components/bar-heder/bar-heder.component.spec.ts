import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BarHederComponent } from './bar-heder.component';

describe('BarHederComponent', () => {
  let component: BarHederComponent;
  let fixture: ComponentFixture<BarHederComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [BarHederComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(BarHederComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
