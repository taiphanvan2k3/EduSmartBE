# ERRORS.md – TypeScript & ESLint Common Errors Reference

> Tài liệu này ghi lại các lỗi TypeScript / ESLint thường gặp trong dự án NestJS này,
> kèm giải thích rõ ràng và cách sửa đúng chuẩn.

---

## 1. `no-unsafe-return` – Trả về kiểu `any` không an toàn

### Lỗi
```
Unsafe return of a value of type `Promise<any>`  @typescript-eslint/no-unsafe-return
```

### Nguyên nhân
`CommandBus.execute()` và `QueryBus.execute()` trong `@nestjs/cqrs` trả về `Promise<any>`.
Khi `return` trực tiếp trong một `async` method, TypeScript báo kiểu trả về không an toàn.

### Ví dụ lỗi
```typescript
async getExportStatus(): Promise<unknown> {
  return this.queryBus.execute(new GetQuery()); // ❌ Promise<any>
}
```

### Fix
Khai báo kiểu trả về là `Promise<unknown>` thay vì để TypeScript tự suy luận:
```typescript
async getExportStatus(): Promise<unknown> {
  return this.queryBus.execute(new GetQuery()); // ✅ Promise<any> ⊂ Promise<unknown>
}
```

> **Tại sao `unknown` ổn?**: `any` có thể gán cho `unknown`, không cần cast.

---

## 2. `no-unsafe-assignment` – Gán giá trị kiểu `any` vào biến

### Lỗi
```
Unsafe assignment of an `any` value  @typescript-eslint/no-unsafe-assignment
```

### Nguyên nhân
Khi dùng `await` với một hàm trả về `Promise<any>`, giá trị nhận về có kiểu `any`.
Gán `any` vào biến không có kiểu tường minh bị lint bắt.

### Ví dụ lỗi
```typescript
const template = await this.queryBus.execute(new GetQuery()); // ❌ kiểu any
```

### Fix – Dùng `.then()` để tránh intermediate variable
```typescript
// Thay vì await + gán biến, chain .then() và đổi tên param thành unknown:
return this.queryBus
  .execute(new GetQuery())
  .then((template: unknown) => ({ courseTemplate: template })); // ✅
```

---

## 3. `no-unnecessary-type-assertion` – Type assertion thừa trên kiểu `any`

### Lỗi
```
This assertion is unnecessary since it does not change the type of the expression.
@typescript-eslint/no-unnecessary-type-assertion
```

### Nguyên nhân
`any as SomeType` không thực sự thay đổi kiểu trong runtime vì `any` đã tự động
chuyển đổi được sang mọi kiểu. TypeScript coi assertion này là **vô nghĩa**.

### Ví dụ lỗi
```typescript
const result = await this.queryBus.execute(new MyQuery()) as MyType; // ❌
```

### Fix – Double cast qua `unknown`
```typescript
// Cast qua unknown trước, rồi sang kiểu cụ thể:
const result = (await this.queryBus.execute(new MyQuery())) as unknown as MyType; // ✅
```

> **Tại sao `as unknown as T` lại ổn?**: `as unknown` không bị coi là thừa vì nó
> thu hẹp `any` thành `unknown`. Sau đó `as T` mới thu hẹp tiếp từ `unknown` sang `T`.

---

## 4. `no-unused-vars` – Parameter không dùng đến

### Lỗi
```
'_query' is defined but never used  @typescript-eslint/no-unused-vars
```

### Nguyên nhân
Interface `IQueryHandler<T>` yêu cầu method `execute(query: T)` có tham số.
Nhưng một số Query không chứa data (ví dụ: `GetAllTemplatesQuery` là class rỗng),
nên tham số không được dùng trong thân hàm.

### Ví dụ lỗi
```typescript
// GetAllTemplatesQuery là class rỗng → không cần đọc nội dung query
async execute(_query: GetAllTemplatesQuery) { // ❌ _query bị báo unused
  return await this.repo.find();
}
```

### Fix – Dùng tên `_` (convention chuẩn TypeScript cho intentionally unused)
```typescript
async execute(_: GetAllTemplatesQuery) { // ✅ _ là convention "tôi biết, cố ý bỏ qua"
  return await this.repo.find();
}
```

Thêm vào `eslint.config.mjs` để ignore tất cả param bắt đầu bằng `_`:
```javascript
'@typescript-eslint/no-unused-vars': [
  'error',
  { argsIgnorePattern: '^_', varsIgnorePattern: '^_' },
],
```

---

## 5. `no-floating-promises` – Promise không được await hoặc xử lý

### Lỗi
```
Promises must be awaited, end with a call to .catch, or end with a call
to .then with a rejection handler.  @typescript-eslint/no-floating-promises
```

### Nguyên nhân
Gọi hàm `async` mà không `await` → Promise "lơ lửng", lỗi bên trong sẽ bị nuốt silently.

### Ví dụ lỗi
```typescript
// Trong main.ts:
bootstrap(); // ❌ floating promise
```

### Fix – Dùng `void` để báo "tôi biết, cố ý không await"
```typescript
void bootstrap(); // ✅ explicit ignore
```

Dùng `void` trong background tasks với `setImmediate`:
```typescript
setImmediate(() => {
  void (async () => {          // ✅ wrap IIFE với void
    await someAsyncTask();
  })();
});
```

---

## 6. `no-misused-promises` – Truyền async callback vào nơi không xử lý Promise

### Lỗi
```
Promise returned in function argument where a void return was expected.
@typescript-eslint/no-misused-promises
```

### Nguyên nhân
Một số hàm (ví dụ: callback của Express middleware) kỳ vọng `void`, không phải `Promise`.
Truyền `async` callback vào đó khiến lỗi bị nuốt.

### Fix – Wrap thành sync hoặc dùng void IIFE
```typescript
// ❌ Lỗi:
app.use(async (req, res, next) => { await something(); next(); });

// ✅ Fix: dùng void IIFE bên trong để tường minh
app.use((req, res, next) => {
  void (async () => { await something(); next(); })();
});
```

---

## 7. `no-unsafe-member-access` – Truy cập property trên kiểu `any`

### Lỗi
```
Unsafe member access .message on an `any` value  @typescript-eslint/no-unsafe-member-access
```

### Nguyên nhân
Trong `catch (error)`, mặc định `error` có kiểu `any` → truy cập `.message`, `.stack`
mà không khai báo kiểu thì bị lint bắt.

### Ví dụ lỗi
```typescript
// ❌
catch (error) {
  console.log(error.message); // unsafe member access
}
```

### Fix – Ép kiểu rõ ràng qua `unknown`
```typescript
// ✅
catch (error: unknown) {
  const err = error as Error;
  console.log(err.message);
  console.log(err.stack);
}
```

---

## 8. Cross-module service injection – Anti-pattern trong CQRS

### Vấn đề (lỗi kiến trúc, không phải lint)
```typescript
// ❌ Handler của module A inject Service của module B trực tiếp:
import { CourseTemplateService } from '../../course-template/course-template.service';

@CommandHandler(...)
class GenerateAchievementHandler {
  constructor(private readonly courseTemplateService: CourseTemplateService) {}
  // Tạo coupling chặt giữa 2 module → vi phạm nguyên tắc CQRS
}
```

### Fix – Dùng QueryBus để giao tiếp cross-module
```typescript
// ✅ Handler inject QueryBus, dispatch Query sang module khác:
import { QueryBus } from '@nestjs/cqrs';
import { GetDefaultCourseTemplateQuery } from '../../course-template/queries/...';

@CommandHandler(...)
class GenerateAchievementHandler {
  constructor(private readonly queryBus: QueryBus) {}

  async execute(command: ...) {
    const template = (await this.queryBus.execute(
      new GetDefaultCourseTemplateQuery(courseId),
    )) as unknown as CourseTemplateResult;
  }
}
```

> **Lợi ích**: Các module hoàn toàn **decoupled** – không cần `exports` hay `imports`
> cross-module. `CqrsModule` làm cầu nối qua Bus.

---

## Tổng kết nhanh

| Rule | Fix chính |
|---|---|
| `no-unsafe-return` | Khai báo `Promise<unknown>` trên method |
| `no-unsafe-assignment` | Dùng `.then((x: unknown) => ...)` thay vì `await` + gán biến |
| `no-unnecessary-type-assertion` | `as unknown as T` thay vì `as T` trực tiếp từ `any` |
| `no-unused-vars` | Đặt tên param là `_`, thêm `argsIgnorePattern: '^_'` vào ESLint |
| `no-floating-promises` | Thêm `void` trước promise, hoặc `await` nó |
| `no-unsafe-member-access` | `catch (error: unknown)` + `const err = error as Error` |
| Cross-module injection | Inject `QueryBus`/`CommandBus`, dispatch Query/Command thay vì import Service |
