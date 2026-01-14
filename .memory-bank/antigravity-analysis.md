# 🪐 Phân tích Repository: Antigravity Workspace Template

Dựa trên việc phân tích mã nguồn và tài liệu của repository `study8677/antigravity-workspace-template`, dưới đây là báo cáo chi tiết:

## 1. Tổng quan & Mục đích
Đây là một **template workspace chuyên dụng để xây dựng AI Agent**, được thiết kế đặc biệt để tối ưu hóa khả năng của các IDE thông minh (như Cursor). Mục tiêu chính là biến IDE từ một trình soạn thảo thụ động thành một **"kiến trúc sư chủ động"** (active architect) hỗ trợ lập trình viên.

Triết lý cốt lõi: **"Clone → Rename → Prompt"**. Đơn giản hóa việc thiết lập kiến trúc enterprise phức tạp chỉ bằng vài bước cơ bản.

## 2. Các Tính Năng Chính
*   **🧠 Bộ nhớ vô hạn (Infinite Memory):** Sử dụng kỹ thuật tóm tắt đệ quy (recursive summarization) để nén ngữ cảnh.
*   **🛠️ Công cụ vạn năng (Universal Tools):** Chỉ cần thả các hàm Python vào thư mục `src/tools/`, agent sẽ tự động phát hiện và sử dụng.
*   **📚 Tự động nạp ngữ cảnh (Auto Context):** Các tài liệu trong `.context/` sẽ tự động được đưa vào prompt của AI.
*   **🔌 Hỗ trợ MCP (Model Context Protocol):** Kết nối mượt mà với GitHub, Database, Filesystem thông qua chuẩn MCP.
*   **🤖 Multi-Agent Swarm:** Hỗ trợ mô hình bầy đàn với Router và các Worker chuyên biệt.
*   **📂 Artifact-First:** Quy trình bắt buộc agent phải tạo ra các "Artifact" (kế hoạch, tài liệu) trước khi viết code thực thi.

## 3. Cấu trúc Dự án
*   **`src/`**: Chứa mã nguồn cốt lõi.
    *   `src/tools/`: Nơi chứa các custom tools.
    *   `src/agents/`: Nơi chứa các agent chuyên biệt (nếu dùng Swarm).
*   **`.context/`**: Knowledge base - nơi chứa kiến thức miền (domain knowledge) cho Agent.
*   **`.antigravity/`**: Chứa các "luật" (rules) đặc biệt để điều khiển hành vi của AI trong IDE (chi tiết trong `rules.md`).
*   **`artifacts/`**: Thư mục lưu trữ đầu ra (kế hoạch triển khai, logs, bằng chứng kiểm thử).

## 4. Quy tắc Hoạt động (Rules)
Dự án áp dụng các quy tắc nghiêm ngặt thông qua file `.cursorrules` và `.antigravity/rules.md`:
1.  **Mission-First:** Bắt buộc đọc `mission.md` để hiểu mục tiêu cao nhất.
2.  **Deep Think:** Bắt buộc sử dụng khối `<thought>` để suy luận trước khi giải quyết các tác vụ phức tạp hoặc đưa ra quyết định kiến trúc.
3.  **Quy chuẩn Code:**
    *   Bắt buộc dùng **Type Hints** cho mọi code Python.
    *   Phải có **Docstrings** chuẩn Google.
    *   Sử dụng **Pydantic** cho các cấu trúc dữ liệu.
4.  **An toàn:** Ưu tiên dùng `pip install` trong môi trường ảo, nghiêm cấm các lệnh xóa hệ thống (`rm -rf`) nếu không có sự chấp thuận.

## Kết luận
Repository này cung cấp một bộ khung (scaffold) mạnh mẽ và chuẩn chỉnh để phát triển các ứng dụng AI Agent hiện đại, tận dụng sức mạnh của Gemini và các mô hình ngôn ngữ lớn khác, đồng thời duy trì sự kiểm soát và cấu trúc chặt chẽ.
