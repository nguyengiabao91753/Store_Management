async function logout() {
    try {
        // Xóa thông tin đăng nhập khỏi localStorage TRƯỚC KHI gọi server
        localStorage.removeItem('userId');
        localStorage.removeItem('loginTime');
        
        // Gọi server để đăng xuất (xóa cookie) bằng fetch
        const response = await fetch('/login/logout', {
            method: 'GET',
            credentials: 'include'
        });
        
        // Redirect về login page sau khi logout thành công
        window.location.href = '/login';
        
    } catch (error) {
        console.error('Logout error:', error);
        // Fallback: redirect về login dù có lỗi
        window.location.href = '/login';
    }
}
