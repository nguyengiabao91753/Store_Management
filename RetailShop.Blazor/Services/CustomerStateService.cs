using RetailShop.Blazor.Dtos;

namespace RetailShop.Blazor.Services
{
    public class CustomerStateService
    {
        private CustomerDto? _currentCustomer;
        private bool _isAuthenticated;

        public event Action? OnStateChanged;

        public bool IsAuthenticated => _isAuthenticated;
        public CustomerDto? CurrentCustomer => _currentCustomer;

        public void SetCustomer(CustomerDto customer)
        {
            _currentCustomer = customer;
            _isAuthenticated = true;
            OnStateChanged?.Invoke();
        }

        public void ClearCustomer()
        {
            _currentCustomer = null;
            _isAuthenticated = false;
            OnStateChanged?.Invoke();
        }

        public void UpdateAuthenticationState(bool isAuthenticated, CustomerDto? customer = null)
        {
            _isAuthenticated = isAuthenticated;
            _currentCustomer = customer;
            OnStateChanged?.Invoke();
        }
    }
}
