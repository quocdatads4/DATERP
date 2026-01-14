# Hướng dẫn sử dụng Antigravity Workspace Template

## 1. Khởi động
Sau khi cài đặt xong, bạn làm theo các bước sau để chạy Agent:

1.  **Mở VS Code** (hoặc Cursor) tại thư mục:
    `C:\Users\QuocDat-PC\Documents\GitHub\antigravity-workspace-template`
2.  **Mở Terminal** và kích hoạt (nếu chưa tự động):
    ```powershell
    # Windows
    .\venv\Scripts\activate
    ```
3.  **Điền API Key**:
    - Mở file `.env`
    - Điền key vào dòng `GOOGLE_API_KEY=...` (lấy key tại [aistudio.google.com](https://aistudio.google.com/))
4.  **Chạy Agent**:
    ```powershell
    python src/agent.py
    ```
    Hoặc chạy với câu lệnh cụ thể:
    ```powershell
    python src/agent.py "Viết một hàm Python tính dãy Fibonacci"
    ```

## 2. Cách mở rộng khả năng cho Agent

### 🛠️ Thêm Công cụ (Tools)
Chỉ cần tạo file Python mới trong thư mục `src/tools/`, Agent sẽ tự động nhận diện.
**Ví dụ:** Tạo `src/tools/thoi_tiet.py`:
```python
def get_weather(location: str) -> str:
    """Lấy thông tin thời tiết cho một địa điểm."""
    return f"Thời tiết tại {location} đang nắng đẹp!"
```
Sau khi khởi động lại, Agent sẽ biết cách dùng hàm này.

### 📚 Thêm Ngữ cảnh (Knowledge)
Nếu bạn muốn Agent hiểu về dự án hoặc tài liệu riêng:
1.  Tạo file `.md` hoặc `.txt` trong thư mục `.context/`.
2.  Agent sẽ tự động đọc các file này để trả lời câu hỏi liên quan.

### 🔌 Kết nối với GitHub/Database (MCP)
Cấu hình trong file `mcp_servers.json`. Mặc định template hỗ trợ kết nối GitHub MCP server (cần cài Node.js).
Để bật:
1.  Sửa `.env`: `MCP_ENABLED=true`
2.  Cấu hình `mcp_servers.json` (nếu cần đổi server).

## 3. Quản lý Đầu ra (Artifacts)
Mọi kết quả làm việc của Agent (kế hoạch, code, logs) sẽ được lưu trong thư mục `artifacts/`.
-   **Kế hoạch:** `artifacts/plan_....md`
-   **Logs:** `artifacts/logs/`

## 4. Reset Bộ nhớ
Nếu Agent nhớ sai hoặc bị "loãng" context:
-   Xóa file `agent_memory.json` ở thư mục gốc.
-   Chạy lại Agent.
