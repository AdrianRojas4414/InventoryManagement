import { ComponentFixture, TestBed } from '@angular/core/testing';
import { LoginComponent } from './login.component';
import { RouterTestingModule } from '@angular/router/testing';
import { AuthService } from '../../services/auth.service';
import { of, throwError } from 'rxjs';

class MockAuthService {
  login(username: string, password: string) {
    if (username === 'admin' && password === '123') {
      localStorage.setItem('role', 'Admin');
      return Promise.resolve({ id: 1, username, role: 'Admin' });
    }
    return Promise.reject('Invalid');
  }
}

describe('LoginComponent', () => {
  let component: LoginComponent;
  let fixture: ComponentFixture<LoginComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LoginComponent, RouterTestingModule],
      providers: [{ provide: AuthService, useClass: MockAuthService }]
    }).compileComponents();

    fixture = TestBed.createComponent(LoginComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should set error on invalid login', async () => {
    component.username = 'wrong';
    component.password = 'wrong';
    await component.onSubmit();
    expect(component.error).toBe('Credenciales inválidas o usuario no encontrado');
  });
});
