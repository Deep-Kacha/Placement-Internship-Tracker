// ===== TP Portal Client-Side JavaScript =====
console.log("site.js loaded version 1.1");

// ---- Mobile Menu Toggle (Guest Header) ----
function toggleMobileMenu() {
    const menu = document.getElementById('mobileMenu');
    if (menu) menu.classList.toggle('hidden');
}

// ---- Sidebar Toggle (Student/Admin) ----
function toggleSidebar() {
    const menu = document.getElementById('sidebarMenu');
    if (menu) menu.classList.toggle('hidden');
}

// ---- Password Visibility Toggle ----
function togglePassword(inputId, iconId) {
    const input = document.getElementById(inputId);
    const icon = document.getElementById(iconId);
    if (input.type === 'password') {
        input.type = 'text';
        icon.classList.remove('fa-eye');
        icon.classList.add('fa-eye-slash');
    } else {
        input.type = 'password';
        icon.classList.remove('fa-eye-slash');
        icon.classList.add('fa-eye');
    }
}

// ---- Real-Time Field Validation (matches React onChange behavior) ----
function validateFieldRealtime(fieldType) {
    if (fieldType === 'role') {
        const val = document.getElementById('loginRole')?.value;
        if (!val) showError('roleError', 'Please select a role.');
        else clearError('roleError');
        setBorderError('loginRole', !val);
    } else if (fieldType === 'email' || fieldType === 'loginEmail') {
        const el = document.getElementById('forgotEmail') || document.getElementById('loginEmail');
        if (!el) return;
        const val = el.value;
        const errorId = el.id === 'loginEmail' ? 'emailError' : 'emailError';
        if (!val) { showError(errorId, 'Email is required.'); setBorderError(el.id, true); }
        else if (!/^[^\s@]+@rku\.ac\.in$/.test(val)) { showError(errorId, 'Only @rku.ac.in email addresses are allowed.'); setBorderError(el.id, true); }
        else { clearError(errorId); setBorderError(el.id, false); }
    } else if (fieldType === 'loginPassword') {
        const val = document.getElementById('loginPassword')?.value;
        if (!val) { showError('passwordError', 'Password is required.'); setBorderError('loginPassword', true); }
        else { clearError('passwordError'); setBorderError('loginPassword', false); }
    } else if (fieldType === 'current') {
        const val = document.getElementById('currentPassword')?.value;
        if (!val) { showError('currentError', 'Current password is required.'); setBorderError('currentPassword', true); }
        else { clearError('currentError'); setBorderError('currentPassword', false); }
    } else if (fieldType === 'new') {
        const el = document.getElementById('newPassword');
        if (!el) return;
        const val = el.value;
        if (!val) showError('newError', 'New password is required.');
        else if (val.length < 8) showError('newError', 'Must be at least 8 characters.');
        else if (!/[A-Z]/.test(val)) showError('newError', 'Must contain an uppercase letter.');
        else if (!/[a-z]/.test(val)) showError('newError', 'Must contain a lowercase letter.');
        else if (!/\d/.test(val)) showError('newError', 'Must contain a number.');
        else if (!/[!@#$%^&*(),.?":{}|<>]/.test(val)) showError('newError', 'Must contain a special character.');
        else clearError('newError');
        setBorderError('newPassword', !!document.getElementById('newError')?.textContent);
        // Also re-validate confirm if it's filled
        const confirm = document.getElementById('confirmPassword');
        if (confirm && confirm.value) {
            if (confirm.value !== val) { showError('confirmError', 'Passwords do not match.'); setBorderError('confirmPassword', true); }
            else { clearError('confirmError'); setBorderError('confirmPassword', false); }
        }
    } else if (fieldType === 'confirm') {
        const val = document.getElementById('confirmPassword')?.value;
        const newVal = document.getElementById('newPassword')?.value;
        if (!val) { showError('confirmError', 'Confirm password is required.'); setBorderError('confirmPassword', true); }
        else if (val !== newVal) { showError('confirmError', 'Passwords do not match.'); setBorderError('confirmPassword', true); }
        else { clearError('confirmError'); setBorderError('confirmPassword', false); }
    } else if (fieldType === 'otp') {
        const val = document.getElementById('resetOtp')?.value;
        if (!val) showError('otpError', 'OTP is required.');
        else if (!/^\d{6}$/.test(val)) showError('otpError', 'OTP must be 6 digits.');
        else clearError('otpError');
    } else if (fieldType === 'resetEmail') {
        const val = document.getElementById('resetEmail')?.value;
        if (!val) { showError('resetEmailError', 'Email is required.'); setBorderError('resetEmail', true); }
        else if (!/^[^\s@]+@rku\.ac\.in$/.test(val)) { showError('resetEmailError', 'Only @rku.ac.in email addresses are allowed.'); setBorderError('resetEmail', true); }
        else { clearError('resetEmailError'); setBorderError('resetEmail', false); }
    } else if (fieldType === 'resetNew') {
        const val = document.getElementById('resetNewPassword')?.value;
        if (!val) showError('resetNewError', 'New password is required.');
        else if (val.length < 8) showError('resetNewError', 'Must be at least 8 characters.');
        else clearError('resetNewError');
    } else if (fieldType === 'resetConfirm') {
        const val = document.getElementById('resetConfirmPassword')?.value;
        const newVal = document.getElementById('resetNewPassword')?.value;
        if (!val) showError('resetConfirmError', 'Confirm password is required.');
        else if (val !== newVal) showError('resetConfirmError', 'Passwords do not match.');
        else clearError('resetConfirmError');
    }
}

function setBorderError(inputId, hasError) {
    const el = document.getElementById(inputId);
    if (!el) return;
    if (hasError) {
        el.classList.add('border-red-500');
        el.classList.remove('border-[#e0d8cc]');
    } else {
        el.classList.remove('border-red-500');
        el.classList.add('border-[#e0d8cc]');
    }
}

// ---- Login Validation ----
function validateLogin(e) {
    if (e) e.preventDefault();
    let valid = true;
    const role = document.getElementById('loginRole');
    const email = document.getElementById('loginEmail');
    const password = document.getElementById('loginPassword');

    clearError('roleError');
    clearError('emailError');
    clearError('passwordError');
    clearError('loginError');

    if (!role.value) { showError('roleError', 'Please select a role.'); setBorderError('loginRole', true); valid = false; }
    else { setBorderError('loginRole', false); }

    if (!email.value) {
        showError('emailError', 'Email is required.'); setBorderError('loginEmail', true); valid = false;
    } else if (!/^[^\s@]+@rku\.ac\.in$/.test(email.value)) {
        showError('emailError', 'Only @rku.ac.in emails are allowed.'); setBorderError('loginEmail', true); valid = false;
    } else { setBorderError('loginEmail', false); }

    if (!password.value) { showError('passwordError', 'Password is required.'); setBorderError('loginPassword', true); valid = false; }
    else { setBorderError('loginPassword', false); }

    if (!valid) return false;

    // Static auth check
    if (role.value === 'Student') {
        if (email.value === 'student@rku.ac.in' && password.value === 'Student@123') {
            window.location.href = '/StudDashboard';
            return false;
        } else {
            showError('loginError', 'Invalid Student email or password');
            return false;
        }
    } else if (role.value === 'Admin') {
        if (email.value === 'admin@rku.ac.in' && password.value === 'Admin@123') {
            window.location.href = '/AdminDashboard';
            return false;
        } else {
            showError('loginError', 'Invalid Admin email or password');
            return false;
        }
    }
    return false;
}

// ---- Text Password Visibility Toggle ----
function togglePasswordText(btn, inputId) {
    const input = document.getElementById(inputId);
    if (input.type === 'password') {
        input.type = 'text';
        btn.textContent = 'Hide';
    } else {
        input.type = 'password';
        btn.textContent = 'Show';
    }
}

// ---- Change Password Validation ----
function validateChangePassword(e) {
    if (e) e.preventDefault();
    let valid = true;
    clearError('currentError');
    clearError('newError');
    clearError('confirmError');

    const current = document.getElementById('currentPassword').value;
    const newPass = document.getElementById('newPassword').value;
    const confirm = document.getElementById('confirmPassword').value;

    if (!current) { showError('currentError', 'Current password is required.'); setBorderError('currentPassword', true); valid = false; }
    else { setBorderError('currentPassword', false); }

    if (!newPass) { showError('newError', 'New password is required.'); setBorderError('newPassword', true); valid = false; }
    else if (newPass.length < 8) { showError('newError', 'Must be at least 8 characters.'); setBorderError('newPassword', true); valid = false; }
    else if (!/[A-Z]/.test(newPass)) { showError('newError', 'Must contain an uppercase letter.'); setBorderError('newPassword', true); valid = false; }
    else if (!/[a-z]/.test(newPass)) { showError('newError', 'Must contain a lowercase letter.'); setBorderError('newPassword', true); valid = false; }
    else if (!/\d/.test(newPass)) { showError('newError', 'Must contain a number.'); setBorderError('newPassword', true); valid = false; }
    else if (!/[!@#$%^&*(),.?":{}|<>]/.test(newPass)) { showError('newError', 'Must contain a special character.'); setBorderError('newPassword', true); valid = false; }
    else { setBorderError('newPassword', false); }

    if (!confirm) { showError('confirmError', 'Confirm password is required.'); setBorderError('confirmPassword', true); valid = false; }
    else if (confirm !== newPass) { showError('confirmError', 'Passwords do not match.'); setBorderError('confirmPassword', true); valid = false; }
    else { setBorderError('confirmPassword', false); }

    if (valid) {
        alert('Password successfully verified and updated!');
        document.getElementById('currentPassword').value = '';
        document.getElementById('newPassword').value = '';
        document.getElementById('confirmPassword').value = '';
    }
    return false;
}

// ---- Forgot Password Validation ----
function validateForgotPassword(e) {
    if (e) e.preventDefault();
    let valid = true;
    clearError('emailError');
    clearError('newError');
    clearError('confirmError');

    const email = document.getElementById('forgotEmail').value;
    const newPass = document.getElementById('newPassword').value;
    const confirm = document.getElementById('confirmPassword').value;

    if (!email) { showError('emailError', 'Email is required.'); setBorderError('forgotEmail', true); valid = false; }
    else if (!/^[^\s@]+@rku\.ac\.in$/.test(email)) { showError('emailError', 'Only @rku.ac.in email addresses are allowed.'); setBorderError('forgotEmail', true); valid = false; }
    else { setBorderError('forgotEmail', false); }

    if (!newPass) { showError('newError', 'New password is required.'); setBorderError('newPassword', true); valid = false; }
    else if (newPass.length < 8) { showError('newError', 'Must be at least 8 characters.'); setBorderError('newPassword', true); valid = false; }
    else if (!/[A-Z]/.test(newPass)) { showError('newError', 'Must contain an uppercase letter.'); setBorderError('newPassword', true); valid = false; }
    else if (!/[a-z]/.test(newPass)) { showError('newError', 'Must contain a lowercase letter.'); setBorderError('newPassword', true); valid = false; }
    else if (!/\d/.test(newPass)) { showError('newError', 'Must contain a number.'); setBorderError('newPassword', true); valid = false; }
    else if (!/[!@#$%^&*(),.?":{}|<>]/.test(newPass)) { showError('newError', 'Must contain a special character.'); setBorderError('newPassword', true); valid = false; }
    else { setBorderError('newPassword', false); }

    if (!confirm) { showError('confirmError', 'Confirm password is required.'); setBorderError('confirmPassword', true); valid = false; }
    else if (confirm !== newPass) { showError('confirmError', 'Passwords do not match.'); setBorderError('confirmPassword', true); valid = false; }
    else { setBorderError('confirmPassword', false); }

    if (valid) {
        alert('Password successfully verified and updated!');
        window.location.href = '/Authentication/login';
    }
    return false;
}

// ---- Reset Password Validation ----
function validateResetPassword(e) {
    if (e) e.preventDefault();
    let valid = true;
    clearError('resetNewError');
    clearError('resetConfirmError');

    const newPass = document.getElementById('resetNewPassword')?.value;
    const confirm = document.getElementById('resetConfirmPassword')?.value;

    if (!newPass) { showError('resetNewError', 'New password is required.'); valid = false; }
    else if (newPass.length < 8) { showError('resetNewError', 'Must be at least 8 characters.'); valid = false; }

    if (!confirm) { showError('resetConfirmError', 'Confirm password is required.'); valid = false; }
    else if (confirm !== newPass) { showError('resetConfirmError', 'Passwords do not match.'); valid = false; }

    if (valid) {
        alert('Password reset successful!');
        window.location.href = '/Authentication/login';
    }
    return false;
}

// ---- Real-Time Registration Field Validation (matches React onChange) ----
function validateRegField(fieldId) {
    const el = document.getElementById(fieldId);
    if (!el) return;
    const val = el.value;
    const errId = fieldId + 'Error';

    clearError(errId);
    setBorderError(fieldId, false);

    if (fieldId === 'enrollment') {
        if (!val) { showError(errId, 'Enrollment number is required'); setBorderError(fieldId, true); }
    } else if (fieldId === 'name') {
        if (!val) { showError(errId, 'Full name is required'); setBorderError(fieldId, true); }
    } else if (fieldId === 'branch') {
        if (!val) { showError(errId, 'Branch is required'); setBorderError(fieldId, true); }
    } else if (fieldId === 'semester') {
        if (!val) { showError(errId, 'Semester is required'); setBorderError(fieldId, true); }
    } else if (fieldId === 'universityEmail') {
        if (!val) { showError(errId, 'University email is required'); setBorderError(fieldId, true); }
        else if (!/\S+@\S+\.\S+/.test(val)) { showError(errId, 'Invalid email format'); setBorderError(fieldId, true); }
    } else if (fieldId === 'personalEmail') {
        if (!val) { showError(errId, 'Personal email is required'); setBorderError(fieldId, true); }
        else if (!/\S+@\S+\.\S+/.test(val)) { showError(errId, 'Invalid email format'); setBorderError(fieldId, true); }
    } else if (fieldId === 'password') {
        if (!val) { showError(errId, 'Password is required'); setBorderError(fieldId, true); }
    } else if (fieldId === 'confirmPassword') {
        const pw = document.getElementById('password')?.value;
        if (!val) { showError(errId, 'Please confirm your password'); setBorderError(fieldId, true); }
        else if (val !== pw) { showError(errId, 'Passwords do not match'); setBorderError(fieldId, true); }
    } else if (fieldId === 'mobile') {
        if (!val) { showError(errId, 'Mobile number is required'); setBorderError(fieldId, true); }
        else if (!/^[0-9]{10}$/.test(val)) { showError(errId, 'Mobile must be 10 digits'); setBorderError(fieldId, true); }
    } else if (fieldId === 'tenth') {
        if (!val) { showError(errId, '10th percentage is required'); setBorderError(fieldId, true); }
        else if (val < 0 || val > 100) { showError(errId, 'Must be between 0 and 100'); setBorderError(fieldId, true); }
    } else if (fieldId === 'twelfth') {
        if (!val) { showError(errId, '12th percentage is required'); setBorderError(fieldId, true); }
        else if (val < 0 || val > 100) { showError(errId, 'Must be between 0 and 100'); setBorderError(fieldId, true); }
    } else if (fieldId === 'cgpa') {
        if (!val) { showError(errId, 'CGPA is required'); setBorderError(fieldId, true); }
        else if (val < 0 || val > 10) { showError(errId, 'CGPA must be between 0 and 10'); setBorderError(fieldId, true); }
    } else if (fieldId === 'parentName') {
        if (!val) { showError(errId, 'Parent name is required'); setBorderError(fieldId, true); }
    } else if (fieldId === 'parentEmail') {
        if (!val) { showError(errId, 'Parent email is required'); setBorderError(fieldId, true); }
        else if (!/\S+@\S+\.\S+/.test(val)) { showError(errId, 'Invalid email format'); setBorderError(fieldId, true); }
    } else if (fieldId === 'parentMobile') {
        if (!val) { showError(errId, 'Parent mobile is required'); setBorderError(fieldId, true); }
        else if (!/^[0-9]{10}$/.test(val)) { showError(errId, 'Must be 10 digits'); setBorderError(fieldId, true); }
    } else if (fieldId === 'currentAddress') {
        if (!val) { showError(errId, 'Current address is required'); setBorderError(fieldId, true); }
    } else if (fieldId === 'permanentAddress') {
        if (!val) { showError(errId, 'Permanent address is required'); setBorderError(fieldId, true); }
    } else if (fieldId === 'city') {
        if (!val) { showError(errId, 'City is required'); setBorderError(fieldId, true); }
    } else if (fieldId === 'state') {
        if (!val) { showError(errId, 'State is required'); setBorderError(fieldId, true); }
    } else if (fieldId === 'pincode') {
        if (!val) { showError(errId, 'Pincode is required'); setBorderError(fieldId, true); }
        else if (!/^[0-9]{6}$/.test(val)) { showError(errId, 'Pincode must be 6 digits'); setBorderError(fieldId, true); }
    }
}

// ---- Real-Time Company Field Validation (matches React onChange) ----
function validateCompField(fieldId) {
    const el = document.getElementById(fieldId);
    if (!el && fieldId !== 'batch' && fieldId !== 'course') return;
    const val = el ? el.value.trim() : '';
    const errId = fieldId + 'Error';

    clearError(errId);
    if (el) setBorderError(fieldId, false);

    const isEmail = email => /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email);
    const isUrl = url => /^(https?:\/\/)?([\da-z\.-]+)\.([a-z\.]{2,6})([\/\w \.-]*)*\/?$/.test(url);
    const isNumeric = val => !isNaN(val) && !isNaN(parseFloat(val));

    let msg = '';
    
    switch (fieldId) {
        case 'compEmail': msg = !val ? 'Email is required' : (!isEmail(val) ? 'Invalid Email' : ''); break;
        case 'compName': msg = !val ? 'Company Name is required' : ''; break;
        case 'compLocation': msg = !val ? 'Job Location is required' : ''; break;
        case 'compWebsite': msg = !val ? 'Website URL is required' : (!isUrl(val) ? 'Invalid URL' : ''); break;
        case 'compDesc': msg = (val && val.length < 10) ? 'Description should be at least 10 characters' : ''; break;
        case 'jobPos': msg = !val ? 'Job Position is required' : ''; break;
        case 'jobCtc': msg = !val ? 'Annual CTC is required' : (!isNumeric(val) ? 'Must be numeric' : ''); break;
        case 'jobInternship': msg = (val && !isNumeric(val)) ? 'Must be numeric' : ''; break;
        case 'jobStipend': msg = (val && !isNumeric(val)) ? 'Must be numeric' : ''; break;
        case 'jobDate': msg = !val ? 'Date of Joining is required' : ''; break;
        case 'selProcess': msg = !val ? 'Selection Process is required' : ''; break;
        case 'selDate': msg = !val ? 'Campus Drive Date is required' : ''; break;
        case 'selLink': msg = (val && !isUrl(val)) ? 'Invalid URL' : ''; break;
        case 'contName': msg = !val ? 'Contact Name is required' : ''; break;
        case 'contEmail': msg = !val ? 'Contact Email is required' : (!isEmail(val) ? 'Invalid Email' : ''); break;
        case 'contMobile': msg = !val ? 'Contact Mobile is required' : (!/^\d{10}$/.test(val) ? 'Must be 10 digits' : ''); break;
        case 'batch':
            msg = document.querySelectorAll('.batch-chk:checked').length === 0 ? 'Select at least one batch' : '';
            break;
        case 'course':
            msg = document.querySelectorAll('.course-chk:checked').length === 0 ? 'Select at least one course' : '';
            break;
        case 'uploadDoc':
            const fileInput = document.getElementById('uploadDoc');
            if (fileInput && fileInput.files.length > 0) {
                const file = fileInput.files[0];
                const allowedTypes = ['application/pdf', 'application/msword', 'application/vnd.openxmlformats-officedocument.wordprocessingml.document'];
                if (!allowedTypes.includes(file.type)) msg = 'Must be PDF or DOC/DOCX';
                else if (file.size > 10 * 1024 * 1024) msg = 'Size must not exceed 10MB';
            }
            break;
    }

    if (msg) {
        showError(errId, msg);
        if (el) setBorderError(fieldId, true);
    }
    return msg === '';
}

// ---- Student Registration Validation ----
function validateStudentRegistration(e) {
    if (e) e.preventDefault();
    let valid = true;
    const fields = [
        { id: 'enrollment', msg: 'Enrollment number is required' },
        { id: 'name', msg: 'Full name is required' },
        { id: 'branch', msg: 'Branch is required' },
        { id: 'semester', msg: 'Semester is required' },
        { id: 'universityEmail', msg: 'University email is required' },
        { id: 'personalEmail', msg: 'Personal email is required' },
        { id: 'password', msg: 'Password is required' },
        { id: 'confirmPassword', msg: 'Please confirm your password' },
        { id: 'mobile', msg: 'Mobile number is required' },
        { id: 'tenth', msg: '10th percentage is required' },
        { id: 'twelfth', msg: '12th percentage is required' },
        { id: 'cgpa', msg: 'CGPA is required' },
        { id: 'parentName', msg: 'Parent name is required' },
        { id: 'parentEmail', msg: 'Parent email is required' },
        { id: 'parentMobile', msg: 'Parent mobile is required' },
        { id: 'currentAddress', msg: 'Current address is required' },
        { id: 'permanentAddress', msg: 'Permanent address is required' },
        { id: 'city', msg: 'City is required' },
        { id: 'state', msg: 'State is required' },
        { id: 'pincode', msg: 'Pincode is required' }
    ];

    fields.forEach(f => {
        clearError(f.id + 'Error');
        const el = document.getElementById(f.id);
        if (el && !el.value) { showError(f.id + 'Error', f.msg); setBorderError(f.id, true); valid = false; }
        else if (el) { setBorderError(f.id, false); }
    });

    const universityEmail = document.getElementById('universityEmail');
    if (universityEmail && universityEmail.value && !/^[^\s@]+@rku\.ac\.in$/.test(universityEmail.value)) {
        showError('universityEmailError', 'Only @rku.ac.in emails are allowed.'); setBorderError('universityEmail', true); valid = false;
    }

    const password = document.getElementById('password');
    const confirmPassword = document.getElementById('confirmPassword');
    if (password && confirmPassword && password.value && confirmPassword.value && password.value !== confirmPassword.value) {
        showError('confirmPasswordError', 'Passwords do not match'); setBorderError('confirmPassword', true); valid = false;
    }

    const mobile = document.getElementById('mobile');
    if (mobile && mobile.value && !/^[0-9]{10}$/.test(mobile.value)) {
        showError('mobileError', 'Mobile must be 10 digits'); setBorderError('mobile', true); valid = false;
    }

    const pincode = document.getElementById('pincode');
    if (pincode && pincode.value && !/^[0-9]{6}$/.test(pincode.value)) {
        showError('pincodeError', 'Pincode must be 6 digits'); setBorderError('pincode', true); valid = false;
    }

    if (valid) {
        alert('Registration submitted successfully! You will receive an approval email.');
        window.location.href = '/Authentication/login';
    }
    return false;
}

// ---- Company Registration Validation ----
function validateCompanyRegistration(e) {
    if (e) e.preventDefault();
    
    let valid = true;
    const fields = [
        'compEmail', 'compName', 'compLocation', 'compWebsite', 'compDesc', 
        'jobPos', 'jobCtc', 'jobInternship', 'jobStipend', 'jobDate', 
        'batch', 'course', 'selProcess', 'selDate', 'selLink', 
        'contName', 'contEmail', 'contMobile', 'uploadDoc'
    ];

    fields.forEach(f => {
        if (!validateCompField(f)) {
            valid = false;
        }
    });

    if (valid) {
        alert('JD submitted successfully! Our T&P team will review it.');
        window.location.href = '/';
    }
    return false;
}

function clearCompanyRegistration() {
    document.getElementById('jdForm')?.reset();
    const fields = [
        'compEmail', 'compName', 'compLocation', 'compWebsite', 'compDesc', 
        'jobPos', 'jobCtc', 'jobInternship', 'jobStipend', 'jobDate', 
        'selProcess', 'selDate', 'selLink', 
        'contName', 'contEmail', 'contMobile', 'uploadDoc',
        'batch', 'course'
    ];
    fields.forEach(f => {
        clearError(f + 'Error');
        setBorderError(f, false);
    });
}

// ---- Search / Filter ----
function filterCards() {
    const searchTerm = (document.getElementById('searchInput')?.value || '').toLowerCase();
    const searchCategory = document.getElementById('searchCategory')?.value || 'All';
    const filterStatus = document.getElementById('filterStatus')?.value || 'All';
    const cards = document.querySelectorAll('.filterable-card');

    cards.forEach(card => {
        const name = (card.dataset.name || '').toLowerCase();
        const role = (card.dataset.role || '').toLowerCase();
        const location = (card.dataset.location || '').toLowerCase();
        const salary = (card.dataset.salary || '').toLowerCase();
        const status = card.dataset.status || '';

        let statusMatch = filterStatus === 'All' || status === filterStatus;
        if (!statusMatch) { card.style.display = 'none'; return; }

        let textMatch = true;
        if (searchTerm) {
            if (searchCategory === 'All') {
                textMatch = name.includes(searchTerm) || role.includes(searchTerm) || location.includes(searchTerm) || salary.includes(searchTerm);
            } else if (searchCategory === 'Company') {
                textMatch = name.includes(searchTerm);
            } else if (searchCategory === 'Role') {
                textMatch = role.includes(searchTerm);
            } else if (searchCategory === 'Location') {
                textMatch = location.includes(searchTerm);
            } else if (searchCategory === 'Salary') {
                textMatch = salary.includes(searchTerm);
            }
        }

        card.style.display = textMatch ? '' : 'none';
    });
}

// ---- Student Search / Filter (Table) ----
function filterStudentTable() {
    const search = (document.getElementById('studentSearchInput')?.value || '').toLowerCase();
    const rows = document.querySelectorAll('.student-row');
    rows.forEach(row => {
        const text = row.textContent.toLowerCase();
        row.style.display = text.includes(search) ? '' : 'none';
    });
}

// ---- Company Status Management ----
function changeCompanyStatus(cardId, newStatus) {
    const card = document.getElementById(cardId);
    if (!card) return;
    
    // The exact React UI has two button sets:
    const statusContainer = card.querySelector('.status-buttons-container');
    const undoContainer = card.querySelector('.undo-container');
    const statusIndicator = card.querySelector('.status-indicator');

    if (statusContainer && undoContainer && statusIndicator) {
        statusContainer.classList.add('hidden');
        undoContainer.classList.remove('hidden');

        if (newStatus === 'verified') {
            statusIndicator.innerHTML = '✓ ACCEPTED';
            statusIndicator.className = 'status-indicator bg-green-600 flex-1 text-white py-2 rounded border-none font-semibold cursor-not-allowed';
        } else if (newStatus === 'rejected') {
            statusIndicator.innerHTML = '✕ DECLINED';
            statusIndicator.className = 'status-indicator bg-red-600 flex-1 text-white py-2 rounded border-none font-semibold cursor-not-allowed';
        }
    }
    
    card.dataset.status = newStatus;
    
    // Store previous status for undo (assuming pending)
    card.dataset.prevStatus = "pending";
}

function undoCompanyStatus(cardId) {
    const card = document.getElementById(cardId);
    if (!card) return;
    
    const statusContainer = card.querySelector('.status-buttons-container');
    const undoContainer = card.querySelector('.undo-container');
    
    if (statusContainer && undoContainer) {
        statusContainer.classList.remove('hidden');
        undoContainer.classList.add('hidden');
    }
    
    card.dataset.status = card.dataset.prevStatus || 'pending';
}

// ---- Student Profile Edit Toggle ----
function toggleProfileEdit() {
    const btn = document.getElementById('editProfileBtn');
    const isEditing = btn.dataset.editing === 'true';
    const inputs = document.querySelectorAll('.profile-editable');
    const avatar = document.querySelector('.w-24.h-24.bg-\\[\\#e7000b\\]');

    if (isEditing) {
        // Save Mode
        if (!validateStudentProfile()) return;

        inputs.forEach(input => {
            input.setAttribute('disabled', 'disabled');
            input.classList.remove('bg-white', 'ring-2', 'ring-red-600');
            input.classList.add('bg-gray-100', 'opacity-95');

            // Update display elements (Identity Card)
            if (input.id === 'studFullNameInput') {
                const nameDisplay = document.getElementById('studFullNameDisplay');
                if (nameDisplay) nameDisplay.textContent = input.value;
                if (avatar) {
                    const parts = input.value.trim().split(/\s+/);
                    let initials = "";
                    if (parts.length === 1) initials = parts[0].charAt(0).toUpperCase();
                    else if (parts.length > 1) initials = (parts[0].charAt(0) + parts[parts.length - 1].charAt(0)).toUpperCase();
                    avatar.textContent = initials;
                }
            }
        });

        btn.innerHTML = '<i class="fas fa-edit mr-2"></i> EDIT CONTACT';
        btn.dataset.editing = 'false';
        alert('Profile saved successfully!');
    } else {
        // Edit Mode
        inputs.forEach(input => {
            input.removeAttribute('disabled');
            input.classList.remove('bg-gray-100', 'opacity-95');
            input.classList.add('bg-white', 'ring-2', 'ring-red-600');
        });

        btn.innerHTML = '<i class="fas fa-save mr-2"></i> SAVE PROFILE';
        btn.dataset.editing = 'true';
    }
}

function validateStudentProfileField(fieldId) {
    const input = document.getElementById(fieldId);
    if (!input) return true;
    const val = input.value.trim();
    const errorId = fieldId + 'Error';
    let msg = '';

    if (fieldId === 'studFullNameInput' && !val) msg = 'Full Name is required';
    else if (fieldId === 'studEmailInput' && !val) msg = 'Email is required';
    else if (fieldId === 'studEmailInput' && !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(val)) msg = 'Invalid email format';
    else if (fieldId === 'studMobileInput' && !val) msg = 'Mobile number is required';
    else if (fieldId === 'studMobileInput' && !/^\d{10}$/.test(val.replace(/\D/g, ''))) msg = 'Invalid mobile number';

    if (msg) {
        showError(errorId, msg);
        input.classList.add('ring-2', 'ring-red-500');
    } else {
        clearError(errorId);
        input.classList.remove('ring-red-500');
        input.classList.add('ring-red-600');
    }
    return !msg;
}

function validateStudentProfile() {
    let isValid = true;
    const fields = ['studFullNameInput', 'studEmailInput', 'studMobileInput'];
    fields.forEach(f => {
        if (!validateStudentProfileField(f)) isValid = false;
    });
    return isValid;
}

// ---- Admin Profile Edit Toggle ----
function toggleAdminProfileEdit() {
    console.log("toggleAdminProfileEdit called");
    const btn = document.getElementById('editAdminProfileBtn');
    if (!btn) { console.error("Button editAdminProfileBtn not found!"); return; }
    const isEditing = btn.dataset.editing === 'true';
    console.log("Current isEditing state:", isEditing);
    const inputs = document.querySelectorAll('.admin-profile-input');
    const displays = document.querySelectorAll('.admin-profile-display');

    if (isEditing) {
        // Save Mode
        if (!validateAdminProfile()) return;

        inputs.forEach(input => {
            const displayId = input.id.replace('Input', 'Display');
            const display = document.getElementById(displayId);
            if (display) {
                display.textContent = input.value;
                display.classList.remove('hidden');
            }

            // Handle Initials Avatar if name changed
            if (input.id === 'adminNameInput') {
                const avatar = document.getElementById('adminInitialsDisplay');
                if (avatar) {
                    const parts = input.value.trim().split(/\s+/);
                    let initials = "";
                    if (parts.length === 1) initials = parts[0].charAt(0).toUpperCase();
                    else if (parts.length > 1) initials = (parts[0].charAt(0) + parts[parts.length - 1].charAt(0)).toUpperCase();
                    avatar.textContent = initials;
                }
            }
            
            input.classList.add('hidden');

            // Restore readonly and styles for info form inputs
            if (input.classList.contains('bg-white') && input.classList.contains('border-gray-400')) {
                input.setAttribute('readonly', 'readonly');
                input.classList.remove('bg-white', 'border-gray-400');
                input.classList.add('bg-gray-200', 'border-transparent');
            }
        });

        btn.innerHTML = '<i class="fas fa-edit"></i> EDIT PROFILE';
        btn.classList.remove('bg-green-600', 'hover:bg-green-700');
        btn.classList.add('bg-red-600', 'hover:bg-red-700');
        btn.dataset.editing = 'false';
        alert('Admin profile updated successfully!');
    } else {
        // Edit Mode
        inputs.forEach(input => {
            const displayId = input.id.replace('Input', 'Display');
            const display = document.getElementById(displayId);
            if (display) {
                display.classList.add('hidden');
            }
            input.classList.remove('hidden');

            // Handle those that are always visible but toggled by readonly
            if (input.hasAttribute('readonly')) {
                input.removeAttribute('readonly');
                input.classList.remove('bg-gray-200', 'border-transparent');
                input.classList.add('bg-white', 'border-gray-400');
            }
        });

        btn.innerHTML = '<i class="fas fa-save"></i> SAVE PROFILE';
        btn.classList.remove('bg-red-600', 'hover:bg-red-700');
        btn.classList.add('bg-green-600', 'hover:bg-green-700');
        btn.dataset.editing = 'true';
    }
}

function validateAdminField(fieldId) {
    const input = document.getElementById(fieldId);
    const val = input.value.trim();
    const errorId = fieldId.replace('Input', 'Error');
    let msg = '';

    if (fieldId === 'adminNameInput' && !val) msg = 'Full Name is required';
    else if (fieldId === 'adminRoleInput' && !val) msg = 'Role is required';
    else if (fieldId === 'adminEmailInput' && !val) msg = 'Email is required';
    else if (fieldId === 'adminEmailInput' && !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(val)) msg = 'Invalid email address';
    else if (fieldId === 'adminMobileInput' && !val) msg = 'Mobile number is required';
    else if (fieldId === 'adminEmpIdInput' && !val) msg = 'Employee Id is required';
    else if (fieldId === 'adminDeptInput' && !val) msg = 'Department is required';
    else if (fieldId === 'adminLocInput' && !val) msg = 'Office Location is required';

    if (msg) {
        showError(errorId, msg);
        input.classList.add('border-red-500');
    } else {
        clearError(errorId);
        input.classList.remove('border-red-500');
        input.classList.add('border-gray-400');
    }
    return !msg;
}

function validateAdminProfile() {
    let isValid = true;
    const fields = [
        'adminNameInput', 'adminRoleInput', 'adminEmailInput', 
        'adminMobileInput', 'adminEmpIdInput', 'adminDeptInput', 'adminLocInput'
    ];
    fields.forEach(f => {
        if (!validateAdminField(f)) isValid = false;
    });
    return isValid;
}

// ---- Job Apply ----
function applyJob(btnId) {
    const btn = document.getElementById(btnId);
    if (btn) {
        btn.textContent = 'Applied';
        btn.className = 'btn-disabled flex-1';
        btn.disabled = true;
        alert('Application submitted successfully!');
    }
}

// ---- Student Delete ----
function deleteStudent(rowId) {
    const row = document.getElementById(rowId);
    if (row && confirm('Are you sure you want to delete this student?')) {
        row.remove();
    }
}

// ---- Interview On Hold Toggle ----
function toggleOnHold(btn) {
    const isHold = btn.dataset.hold === 'true';
    if (isHold) {
        btn.dataset.hold = 'false';
        btn.classList.remove('bg-gray-500', 'hover:bg-gray-600');
        btn.classList.add('bg-orange-500', 'hover:bg-orange-600');
    } else {
        btn.dataset.hold = 'true';
        btn.classList.remove('bg-orange-500', 'hover:bg-orange-600');
        btn.classList.add('bg-gray-500', 'hover:bg-gray-600');
    }
}

// ---- JD Detail Edit Toggle ----
function toggleJdEdit() {
    const inputs = document.querySelectorAll('.jd-editable');
    const btn = document.getElementById('editJdBtn');
    const saveArea = document.getElementById('jdSaveArea');
    const isEditing = btn.dataset.editing === 'true';

    inputs.forEach(input => {
        if (isEditing) {
            input.setAttribute('readonly', 'readonly');
            input.classList.add('bg-gray-100', 'text-gray-500');
            input.classList.remove('focus:ring-2', 'focus:ring-red-400');
        } else {
            input.removeAttribute('readonly');
            input.classList.remove('bg-gray-100', 'text-gray-500');
            input.classList.add('focus:ring-2', 'focus:ring-red-400');
        }
    });

    if (isEditing) {
        btn.disabled = false;
        btn.classList.remove('opacity-50', 'cursor-not-allowed');
        btn.dataset.editing = 'false';
        if (saveArea) saveArea.classList.add('hidden');
    } else {
        btn.disabled = true;
        btn.classList.add('opacity-50', 'cursor-not-allowed');
        btn.dataset.editing = 'true';
        if (saveArea) saveArea.classList.remove('hidden');
    }
}

function cancelJdEdit() {
    const btn = document.getElementById('editJdBtn');
    btn.dataset.editing = 'true';
    toggleJdEdit();
}

function saveJdChanges() {
    alert('JD changes saved successfully!');
    cancelJdEdit();
}

// ---- Utility Functions ----
function showError(id, msg) {
    const el = document.getElementById(id);
    if (el) { el.textContent = msg; el.classList.remove('hidden'); }
}

function clearError(id) {
    const el = document.getElementById(id);
    if (el) { el.textContent = ''; el.classList.add('hidden'); }
}

function clearForm(formId) {
    const form = document.getElementById(formId);
    if (form) form.reset();
    form?.querySelectorAll('.text-red-500').forEach(el => { el.textContent = ''; el.classList.add('hidden'); });
    form?.querySelectorAll('.border-red-500').forEach(el => { el.classList.remove('border-red-500'); });
}

// ---- Active Sidebar Highlight ----
document.addEventListener('DOMContentLoaded', function () {
    const path = window.location.pathname;
    document.querySelectorAll('.sidebar-link').forEach(link => {
        const href = link.getAttribute('href');
        if (href === path) {
            link.classList.add('active');
            link.classList.remove('bg-white');
        }
        // Sub-page highlights
        if (href === '/StudJobs' && path === '/StudJobDetails') { link.classList.add('active'); link.classList.remove('bg-white'); }
        if (href === '/StudChangePass' && path === '/StudForgotPassword') { link.classList.add('active'); link.classList.remove('bg-white'); }
        if (href === '/AdminCompanyManagement' && path === '/AdminJdDetail') { link.classList.add('active'); link.classList.remove('bg-white'); }
        if (href === '/AdminInterviewSchedule' && path === '/AdminInterviewScheduleForm') { link.classList.add('active'); link.classList.remove('bg-white'); }
        if (href === '/AdminChangePassword' && path === '/AdminForgotPassword') { link.classList.add('active'); link.classList.remove('bg-white'); }
    });

    // Smooth scroll for anchor links on home page
    document.querySelectorAll('a[href^="/#"]').forEach(link => {
        link.addEventListener('click', function(e) {
            if (window.location.pathname === '/') {
                e.preventDefault();
                const id = this.getAttribute('href').replace('/#', '');
                const target = document.getElementById(id);
                if (target) target.scrollIntoView({ behavior: 'smooth' });
            }
        });
    });
});

