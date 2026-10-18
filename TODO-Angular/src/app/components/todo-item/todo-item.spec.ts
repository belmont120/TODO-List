import { ComponentFixture, TestBed } from '@angular/core/testing';
import { TodoItem } from './todo-item';

describe('TodoItem', () => {
  let component: TodoItem;
  let fixture: ComponentFixture<TodoItem>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TodoItem],
    }).compileComponents();

    fixture = TestBed.createComponent(TodoItem);
    fixture.componentRef.setInput('todo', {
      id: '1',
      title: 'Test todo',
      completed: false,
      createdAt: Date.now(),
    });
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
