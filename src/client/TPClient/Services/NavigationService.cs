using System;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;

namespace TPClient.Services
{
    public class NavigationService : INavigationService
    {
        private readonly Frame _frame;

        public NavigationService(Frame frame)
        {
            _frame = frame;
        }

        public void GoBack()
        {
            _frame.GoBack();
        }

        public void GoForward()
        {
            _frame.GoForward();
        }

        public bool Navigate<T>(object parameter = null)
        {
            var type = typeof(T);
            return Navigate(type, parameter);
        }

        public bool Navigate(Type source, object parameter = null)
        {
            var src = Activator.CreateInstance(source);
            return _frame.Navigate(src, parameter);
        }

        public bool Navigate(string page)
        {
            var type = Assembly.GetExecutingAssembly().GetTypes().SingleOrDefault(a => a.Name.Equals(page));
            if (type == null) return false;

            var src = Activator.CreateInstance(type);
            return _frame.Navigate(src);
        }
    }
}
