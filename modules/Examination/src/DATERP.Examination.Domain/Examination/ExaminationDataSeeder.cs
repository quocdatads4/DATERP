using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;

namespace DATERP.Examination.Examination;

public class ExaminationDataSeeder : IDataSeedContributor, ITransientDependency
{
    private readonly IRepository<ExamSubject, Guid> _subjectRepository;
    private readonly IRepository<ExamList, Guid> _listRepository;
    private readonly IRepository<ExamProject, Guid> _projectRepository;
    private readonly IRepository<ExamTask, Guid> _taskRepository;
    private readonly IGuidGenerator _guidGenerator;

    public ExaminationDataSeeder(
        IRepository<ExamSubject, Guid> subjectRepository,
        IRepository<ExamList, Guid> listRepository,
        IRepository<ExamProject, Guid> projectRepository,
        IRepository<ExamTask, Guid> taskRepository,
        IGuidGenerator guidGenerator)
    {
        _subjectRepository = subjectRepository;
        _listRepository = listRepository;
        _projectRepository = projectRepository;
        _taskRepository = taskRepository;
        _guidGenerator = guidGenerator;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        var subjects = new[]
        {
            new { Name = "Word 2019", Code = "WORD2019" },
            new { Name = "Excel 2019", Code = "EXCEL2019" },
            new { Name = "PowerPoint 2019", Code = "PPT2019" }
        };

        foreach (var sub in subjects)
        {
            var existingSubject = await _subjectRepository.FindAsync(x => x.Code == sub.Code);
            if (existingSubject == null)
            {
                var subject = new ExamSubject(_guidGenerator.Create(), sub.Name, sub.Code, $"Chứng chỉ {sub.Name} quốc tế");
                await _subjectRepository.InsertAsync(subject);

                // Create default lists and projects for new subject
                for (int i = 1; i <= 3; i++)
                {
                    var list = new ExamList(_guidGenerator.Create(), subject.Id, $"{sub.Name} - Bộ đề {i}", 50, i);
                    await _listRepository.InsertAsync(list);

                    for (int j = 1; j <= 6; j++)
                    {
                        var project = new ExamProject(_guidGenerator.Create(), list.Id, $"Project {j}", $"Nội dung thực hành cho Project {j}", j);
                        await _projectRepository.InsertAsync(project);

                        for (int k = 1; k <= 5; k++)
                        {
                            var task = new ExamTask(_guidGenerator.Create(), project.Id, $"Task {k}: Yêu cầu cụ thể...", 10, k);
                            await _taskRepository.InsertAsync(task);
                        }
                    }
                }
            }
        }

        // Specific data update for Project 1 - Word 2019 (requested by user)
        var wordSubject = await _subjectRepository.FindAsync(x => x.Code == "WORD2019");
        if (wordSubject != null)
        {
            // Note: Title matches the one generated above: $"{sub.Name} - Bộ đề {i}" -> "Word 2019 - Bộ đề 1"
            var wordList1 = await _listRepository.FindAsync(x => x.SubjectId == wordSubject.Id && x.Title == "Word 2019 - Bộ đề 1");
            if (wordList1 != null)
            {
                var project1 = await _projectRepository.FindAsync(x => x.ExamListId == wordList1.Id && x.Order == 1);
                if (project1 != null)
                {
                    var tasksProject1 = new[]
                    {
                        new { Order = 1, Content = "Trong thuộc tính tệp, thêm \"dinosaur\" vào category", GradingConfig = "{\"Type\": \"Property\", \"Key\": \"Category\", \"Value\": \"dinosaur\", \"Match\": \"Contains\"}" },
                        new { Order = 2, Content = "Trong phần \"When and Where Dinosaurs Lived\", sao chép định dạng của đoạn đầu tiên và áp dụng nó cho đoạn thứ hai.", GradingConfig = "{\"Type\": \"ParagraphFormat\", \"ReferenceParagraphOrder\": 1, \"TargetParagraphOrder\": 2, \"HeadingText\": \"When and Where Dinosaurs Lived\"}" },
                        new { Order = 3, Content = "Trong phần \"Geologic eras\", sắp xếp dữ liệu của bảng theo \"Geologic period\" tăng dần (Ascending) và sau đó theo \"Dinosaur\" tăng dần (Ascending).", GradingConfig = "{\"Type\": \"TableSort\", \"HeadingText\": \"Geologic eras\", \"SortColumns\": [{\"ColIndex\": 1, \"Order\": \"Ascending\"}, {\"ColIndex\": 2, \"Order\": \"Ascending\"}]}" },
                        new { Order = 4, Content = "Trong phần \"Ornithischian Dinosaurs\", thay đổi cấp độ danh sách cho \"Developed a narrow eyebrow\" thành level 3.", GradingConfig = "{\"Type\": \"ListLevel\", \"TargetText\": \"Developed a narrow eyebrow\", \"Level\": 2}" },
                        new { Order = 5, Content = "Trong phần \"Kids love dinosaurs\", tại dòng trắng ở cuối trang, sử dụng 3D Models feature để chèn T-Rex model từ thư mục 3D objects. Với text wraping là Square.", GradingConfig = "{\"Type\": \"3DModel\", \"Wrapping\": \"Square\"}" },
                        new { Order = 6, Content = "Trong phần \"Theropod\", áp dụng hiệu ứng nghệ thuật (artistic effect) Pencil Sketch cho bức tranh hóa thạch khủng long.", GradingConfig = "{\"Type\": \"ArtisticEffect\", \"EffectName\": \"PencilSketch\", \"ContextText\": \"Theropod\"}" }
                    };

                    foreach (var t in tasksProject1)
                    {
                        var existingTask = await _taskRepository.FindAsync(x => x.ProjectId == project1.Id && x.Order == t.Order);
                        if (existingTask != null)
                        {
                            existingTask.Content = t.Content;
                            existingTask.GradingConfig = t.GradingConfig; // Update Config
                            await _taskRepository.UpdateAsync(existingTask);
                        }
                        else
                        {
                            var newTask = new ExamTask(_guidGenerator.Create(), project1.Id, t.Content, 10, t.Order, t.GradingConfig);
                            await _taskRepository.InsertAsync(newTask);
                        }
                    }
                }

                // Project 2 - Word 2019
                var project2 = await _projectRepository.FindAsync(x => x.ExamListId == wordList1.Id && x.Order == 2);
                if (project2 != null)
                {
                    var tasksProject2 = new[]
                    {
                        new { Order = 1, Content = "Hiển thị Integral header trên tất cả các trang của tài liệu ngoại trừ trang 1" },
                        new { Order = 2, Content = "Trong phần \"Depanning\", chèn biểu tượng thermometer trước cụm từ \"The muffin tray will still be hot!\". Sử dụng font Webdings và mã ký tự \"225\" (biểu tượng nhiệt kế)." },
                        new { Order = 3, Content = "Đặt khoảng cách giãn dòng thành 1.3 lines cho toàn bộ tài liệu." },
                        new { Order = 4, Content = "Trong phần \"Top Sellers\", tiếp tục đánh số danh sách ở đầu cột thứ hai, để các mục danh sách được đánh số từ 1 đến 6." },
                        new { Order = 5, Content = "Trong phần \"Overview\", hãy áp dụng hiệu ứng hình vát tròn mềm (Soft Round bevel) cho đồ họa SmartArt. (Hãy chắc chắn chọn toàn bộ đồ họa SmartArt)." }
                    };

                    foreach (var t in tasksProject2)
                    {
                        var existingTask = await _taskRepository.FindAsync(x => x.ProjectId == project2.Id && x.Order == t.Order);
                        if (existingTask != null)
                        {
                            existingTask.Content = t.Content;
                            await _taskRepository.UpdateAsync(existingTask);
                        }
                        else
                        {
                            var newTask = new ExamTask(_guidGenerator.Create(), project2.Id, t.Content, 10, t.Order);
                            await _taskRepository.InsertAsync(newTask);
                        }
                    }
                }

                // Project 3 - Word 2019
                var project3 = await _projectRepository.FindAsync(x => x.ExamListId == wordList1.Id && x.Order == 3);
                if (project3 != null)
                {
                    var tasksProject3 = new[]
                    {
                        new { Order = 1, Content = "Tìm từ \"national\" và xóa nó khỏi văn bản." },
                        new { Order = 2, Content = "Sử dụng tính năng Word để thay thế tất cả \"HSBC\" bằng \"TMOB\"" },
                        new { Order = 3, Content = "Trong phần \"Banking Fees\", chuyển đổi văn bản được phân tách bằng dấu tab thành một bảng hai cột. Chấp nhận AutoFit mặc định." },
                        new { Order = 4, Content = "Tại dòng trắng sau tiêu đề văn bản, chèn một mục lục. Sử dụng kiểu Automatic Table 1." },
                        new { Order = 5, Content = "Trong phần \"Checking Account\", trong hộp văn bản text box màu xanh đậm, chèn văn bản \"EASY AND FAST\"." },
                        new { Order = 6, Content = "Trong phần \"Fast and convenient\", xóa bình luận đính kèm vào văn bản \"$5,000\"." }
                    };

                    foreach (var t in tasksProject3)
                    {
                        var existingTask = await _taskRepository.FindAsync(x => x.ProjectId == project3.Id && x.Order == t.Order);
                        if (existingTask != null)
                        {
                            existingTask.Content = t.Content;
                            await _taskRepository.UpdateAsync(existingTask);
                        }
                        else
                        {
                            var newTask = new ExamTask(_guidGenerator.Create(), project3.Id, t.Content, 10, t.Order);
                            await _taskRepository.InsertAsync(newTask);
                        }
                    }
                }

                // Project 4 - Word 2019
                var project4 = await _projectRepository.FindAsync(x => x.ExamListId == wordList1.Id && x.Order == 4);
                if (project4 != null)
                {
                    var tasksProject4 = new[]
                    {
                        new { Order = 1, Content = "Dự án này chỉ có 1 nhiệm vụ. Lưu một bản sao của tài liệu vào thư mục Documents của bạn kiểu plain-text với tên file là \"Memo\"." }
                    };

                    foreach (var t in tasksProject4)
                    {
                        var existingTask = await _taskRepository.FindAsync(x => x.ProjectId == project4.Id && x.Order == t.Order);
                        if (existingTask != null)
                        {
                            existingTask.Content = t.Content;
                            await _taskRepository.UpdateAsync(existingTask);
                        }
                        else
                        {
                            var newTask = new ExamTask(_guidGenerator.Create(), project4.Id, t.Content, 10, t.Order);
                            await _taskRepository.InsertAsync(newTask);
                        }
                    }
                }

                // Project 5 - Word 2019
                var project5 = await _projectRepository.FindAsync(x => x.ExamListId == wordList1.Id && x.Order == 5);
                if (project5 != null)
                {
                    var tasksProject5 = new[]
                    {
                        new { Order = 1, Content = "Áp dụng Centered style set cho tài liệu." },
                        new { Order = 2, Content = "Trong phần \"Contact Us\", hợp nhất các ô trong hàng đầu tiên của bảng." },
                        new { Order = 3, Content = "Trong phần \"conference topic\", chuyển đổi năm đoạn bắt đầu bằng \"Applying Psychological Theories to Classroom Instruction\" thành bulleted list. Canh chỉnh đánh dấu đầu dòng ở lề." },
                        new { Order = 4, Content = "Trong phần \"Mission\", chèn cước chú (footnote) bên phải của tiêu đề. Nhập cước chú với nội dung \"Includes digital and hard files\"." },
                        new { Order = 5, Content = "Chấp nhận tất cả thao tác chèn và xóa được theo dõi. Từ chối tất cả các thay đổi định dạng." }
                    };

                    foreach (var t in tasksProject5)
                    {
                        var existingTask = await _taskRepository.FindAsync(x => x.ProjectId == project5.Id && x.Order == t.Order);
                        if (existingTask != null)
                        {
                            existingTask.Content = t.Content;
                            await _taskRepository.UpdateAsync(existingTask);
                        }
                        else
                        {
                            var newTask = new ExamTask(_guidGenerator.Create(), project5.Id, t.Content, 10, t.Order);
                            await _taskRepository.InsertAsync(newTask);
                        }
                    }
                }

                // Project 6 - Word 2019
                var project6 = await _projectRepository.FindAsync(x => x.ExamListId == wordList1.Id && x.Order == 6);
                if (project6 != null)
                {
                    var tasksProject6 = new[]
                    {
                        new { Order = 1, Content = "Thêm một đường viền trang màu xanh lam (Blue, Accent 1, Darker 50%) dày 3 pt kiểu box." },
                        new { Order = 2, Content = "Kiểm tra tài liệu và xóa tất cả các phần headers, footers, và watermarks được tìm thấy. Không xóa thông tin khác." },
                        new { Order = 3, Content = "Ở cuối tài liệu, thay đổi khoảng cách giãn dòng của hai đoạn cuối cùng thành exactly 14 pt." },
                        new { Order = 4, Content = "Áp dụng kiểu Intense Emphasis style cho đoạn văn bản sau hình ảnh." },
                        new { Order = 5, Content = "Chia bốn đoạn văn bản trước hình ảnh thành hai cột với khoảng cách cột là 0.3\"." },
                        new { Order = 6, Content = "Trong phần \"Serving\", thay đổi text wrapping cho hình ảnh thành Square." }
                    };

                    foreach (var t in tasksProject6)
                    {
                        var existingTask = await _taskRepository.FindAsync(x => x.ProjectId == project6.Id && x.Order == t.Order);
                        if (existingTask != null)
                        {
                            existingTask.Content = t.Content;
                            await _taskRepository.UpdateAsync(existingTask);
                        }
                        else
                        {
                            var newTask = new ExamTask(_guidGenerator.Create(), project6.Id, t.Content, 10, t.Order);
                            await _taskRepository.InsertAsync(newTask);
                        }
                    }
                }
            }

            // Word 2019 - Bộ đề 2
            var wordList2 = await _listRepository.FindAsync(x => x.SubjectId == wordSubject.Id && x.Title == "Word 2019 - Bộ đề 2");
            if (wordList2 != null)
            {
                // Project 1 - List 2
                var project1 = await _projectRepository.FindAsync(x => x.ExamListId == wordList2.Id && x.Order == 1);
                if (project1 != null)
                {
                    var tasksProject1 = new[]
                    {
                        new { Order = 1, Content = "Kiểm tra tài liệu để biết vấn đề về khả năng truy cập. Sửa vấn đề liên quan đến bảng được báo cáo trong kết quả kiểm tra bằng cách sử dụng hành động được đề xuất đầu tiên." },
                        new { Order = 2, Content = "Thay đổi hướng (orientation) của trang 3 thành Landscape." },
                        new { Order = 3, Content = "Trong phần \"Fill Material\", thay đổi kích thước bảng sao cho mỗi cột có chiều rộng 2.2\"" },
                        new { Order = 4, Content = "Trong phần \"Description\", chèn một mục trích dẫn giữ chỗ (placeholder citation) với tên \"Manufacturing1\" vào cuối đoạn văn thứ 2 sau tiêu đề." },
                        new { Order = 5, Content = "Trong phần \"Manufacturing Process\", thiết lập alt text description là \"Medicine Process\" cho đồ họa SmartArt. (Hãy chắc chắn chọn toàn bộ đồ họa SmartArt)." },
                        new { Order = 6, Content = "Trong phần \"Description\", sử dụng tính năng 3D-Models để chèn mô hình PILLS từ thư mục 3D Objects vào dòng trống." }
                    };

                    foreach (var t in tasksProject1)
                    {
                        var existingTask = await _taskRepository.FindAsync(x => x.ProjectId == project1.Id && x.Order == t.Order);
                        if (existingTask != null)
                        {
                            existingTask.Content = t.Content;
                            await _taskRepository.UpdateAsync(existingTask);
                        }
                        else
                        {
                            var newTask = new ExamTask(_guidGenerator.Create(), project1.Id, t.Content, 10, t.Order);
                            await _taskRepository.InsertAsync(newTask);
                        }
                    }
                }

                // Project 2 - List 2
                var project2 = await _projectRepository.FindAsync(x => x.ExamListId == wordList2.Id && x.Order == 2);
                if (project2 != null)
                {
                    var tasksProject2 = new[]
                    {
                        new { Order = 1, Content = "Xóa chế độ tương thích (compatibility) của tài liệu này." },
                        new { Order = 2, Content = "Chuyển đổi tất cả các chú thích cuối trang (endnotes) thành cước chú (footnotes)." },
                        new { Order = 3, Content = "Thay đổi lề trang của tài liệu bằng cách đặt lề trên và dưới là 0.5\", lề trái và phải là 0.25\"." },
                        new { Order = 4, Content = "Xóa tất cả định dạng của đoạn văn bản bắt đầu bằng \"Here, we demonstrate various types...embedded fonts.\"" },
                        new { Order = 5, Content = "Trong bảng đầu tiên, đặt khoảng cách là 0.07\"." },
                        new { Order = 6, Content = "Thay đổi mục lục (table of contents) để chỉ hiển thị heading 1." }
                    };

                    foreach (var t in tasksProject2)
                    {
                        var existingTask = await _taskRepository.FindAsync(x => x.ProjectId == project2.Id && x.Order == t.Order);
                        if (existingTask != null)
                        {
                            existingTask.Content = t.Content;
                            await _taskRepository.UpdateAsync(existingTask);
                        }
                        else
                        {
                            var newTask = new ExamTask(_guidGenerator.Create(), project2.Id, t.Content, 10, t.Order);
                            await _taskRepository.InsertAsync(newTask);
                        }
                    }
                }

                // Project 3 - List 2
                var project3 = await _projectRepository.FindAsync(x => x.ExamListId == wordList2.Id && x.Order == 3);
                if (project3 != null)
                {
                    var tasksProject3 = new[]
                    {
                        new { Order = 1, Content = "Ở cuối câu \"Remember, even if you're vaccinated,...\" , thêm một ngắt vùng (section break) kiểu continuous." },
                        new { Order = 2, Content = "Trong phần \"ALTA NORWAY\", chuyển đổi hai đoạn văn thành ba cột." },
                        new { Order = 3, Content = "Trong phần dòng đầu trang (header) của tài liệu, áp dụng Fill: Blue, Accent color 1; Shadow text effect text effect cho văn bản." },
                        new { Order = 4, Content = "Trong phần \"RELATED TOPICS\", thay đổi các bullet points thành bullet points tùy chỉnh. Sử dụng biểu tượng máy bay từ phông chữ Segoe UI Emoji và mã ký tự: \"2708\"." },
                        new { Order = 5, Content = "Trong thuộc tính tệp, thêm \"Top 5 best places\" làm Subject" },
                        new { Order = 6, Content = "Thay đổi nội dung hiển thị của đối tượng đồ họa SmartArt để liệt kê các chủ đề từ \"Cheap travel destinations\" đến \"Top travel destinations\", từ phải sang trái. Không thay đổi thứ tự của các chủ đề." }
                    };

                    foreach (var t in tasksProject3)
                    {
                        var existingTask = await _taskRepository.FindAsync(x => x.ProjectId == project3.Id && x.Order == t.Order);
                        if (existingTask != null)
                        {
                            existingTask.Content = t.Content;
                            await _taskRepository.UpdateAsync(existingTask);
                        }
                        else
                        {
                            var newTask = new ExamTask(_guidGenerator.Create(), project3.Id, t.Content, 10, t.Order);
                            await _taskRepository.InsertAsync(newTask);
                        }
                    }
                }

                // Project 4 - List 2
                var project4 = await _projectRepository.FindAsync(x => x.ExamListId == wordList2.Id && x.Order == 4);
                if (project4 != null)
                {
                    var tasksProject4 = new[]
                    {
                        new { Order = 1, Content = "Trong phần \"Partners\", giải quyết bình luận (resolve the comment)." },
                        new { Order = 2, Content = "Định vị đoạn văn bắt đầu bằng \"Panasonic Energy Company...\", thêm một bookmark ở đầu đoạn này với tên \"main\"" },
                        new { Order = 3, Content = "Ngay dưới đoạn văn bản cuối cùng trên trang 2, chèn một bảng mới có 2 cột và 5 hàng. Ở cột đầu tiên, ở hàng đầu tiên, nhập \"Name\", và cột bên phải nhập \"Country\". Điều chỉnh bảng cho vừa với nội dung của nó." },
                        new { Order = 4, Content = "Cấu hình bảng để dòng đầu tiên được tự động lặp lại ở phía trên mỗi trang như là dòng tiêu đề." },
                        new { Order = 5, Content = "Trong khoảng trống ở cuối của trang cuối cùng, chèn một hình dạng Scroll; Horizontal chứa văn bản \"Good luck!\": Đặt shape ở giữa cuối trang với wrapping dạng tight." },
                        new { Order = 6, Content = "Tìm câu \"Tesla Inc. is an American electric vehicle and clean energy company based in Austin, Texas, United States.\" và áp dụng kiểu Intense Emphasis cho cụm từ này." }
                    };

                    foreach (var t in tasksProject4)
                    {
                        var existingTask = await _taskRepository.FindAsync(x => x.ProjectId == project4.Id && x.Order == t.Order);
                        if (existingTask != null)
                        {
                            existingTask.Content = t.Content;
                            await _taskRepository.UpdateAsync(existingTask);
                        }
                        else
                        {
                            var newTask = new ExamTask(_guidGenerator.Create(), project4.Id, t.Content, 10, t.Order);
                            await _taskRepository.InsertAsync(newTask);
                        }
                    }
                }

                // Project 5 - List 2
                var project5 = await _projectRepository.FindAsync(x => x.ExamListId == wordList2.Id && x.Order == 5);
                if (project5 != null)
                {
                    var tasksProject5 = new[]
                    {
                        new { Order = 1, Content = "Ở đầu tiêu đề \"What are the business applications of social media?\" thêm một ngắt vùng (section break) kiểu next page." },
                        new { Order = 2, Content = "Thay đổi hướng của chỉ trang 1 thành landscape." },
                        new { Order = 3, Content = "Trong phần \"What is social media?\", ở cuối của tiêu đề, chèn một chỗ dành sẵn cho trích dẫn (placeholder citation) mới có tên \"Definition1\"." },
                        new { Order = 4, Content = "Trong sơ đồ SmartArt, nhập alt-text description là \"Basic pie\"" },
                        new { Order = 5, Content = "Thay đổi các đoạn văn bản trong phần \"What are the business applications of social media?\" thành hai cột với đường kẻ giữa (line between)" },
                        new { Order = 6, Content = "Trong phần \"What are the challenges of social media?\", bắt đầu danh sách được đánh số từ \"111\"." }
                    };

                    foreach (var t in tasksProject5)
                    {
                        var existingTask = await _taskRepository.FindAsync(x => x.ProjectId == project5.Id && x.Order == t.Order);
                        if (existingTask != null)
                        {
                            existingTask.Content = t.Content;
                            await _taskRepository.UpdateAsync(existingTask);
                        }
                        else
                        {
                            var newTask = new ExamTask(_guidGenerator.Create(), project5.Id, t.Content, 10, t.Order);
                            await _taskRepository.InsertAsync(newTask);
                        }
                    }
                }

                // Project 6 - List 2
                var project6 = await _projectRepository.FindAsync(x => x.ExamListId == wordList2.Id && x.Order == 6);
                if (project6 != null)
                {
                    var tasksProject6 = new[]
                    {
                        new { Order = 1, Content = "Dự án này chỉ có một nhiệm vụ. Lưu một bản sao của tài liệu dưới dạng Word 2019 template có tên \"Social Media\" tương thích với các tính năng mới nhất của Word và không hỗ trợ Macros. Lưu template file trong thư mục Documents." }
                    };

                    foreach (var t in tasksProject6)
                    {
                        var existingTask = await _taskRepository.FindAsync(x => x.ProjectId == project6.Id && x.Order == t.Order);
                        if (existingTask != null)
                        {
                            existingTask.Content = t.Content;
                            await _taskRepository.UpdateAsync(existingTask);
                        }
                        else
                        {
                            var newTask = new ExamTask(_guidGenerator.Create(), project6.Id, t.Content, 10, t.Order);
                            await _taskRepository.InsertAsync(newTask);
                        }
                    }
                }
            }
        }
    }
}
