
namespace TFramework.ToolBox
{
    // 自定义一个HelloBox类，可以在UI Builder中找到
    public class HelloBox : BaseToolBox
    {
        public override bool PreLoad => true;
        public override bool Closeable => true;

        public override string TabName => "Ciallo\uff5e(\u2220・ω< )\u2312\u2606";

        public override int PreLoadIndex => 0;

        public override string VisualTreeAssetPath => "HelloBox";
        
    }
    
}