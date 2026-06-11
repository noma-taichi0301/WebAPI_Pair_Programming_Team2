//新規登録ボタン押下処理
document.getElementById("addUserButton").addEventListener("click", async () => {

    console.info("新規登録ボタン押下");

    //入力取得
    const userName = document.getElementById("addUserName").value;
    const password = document.getElementById("addPassword").value;

    //入力チェック
    const errorMessage = document.getElementById("errorMessage");
    errorMessage.textContent = "";
    errorMessage.style.display = "none";

    let errors = [];

    if (userName === "") {
        errors.push("ユーザー名を入力してください");
    }

    if (userName.length > 30) {
        errors.push("ユーザー名は30文字以下にしてください");
    }

    if (password === "") {
        errors.push("パスワードを入力してください");
    }else if (password.length > 30 || password.length < 8) {
        errors.push("パスワードは8文字以上、30文字以下にしてください");
    }

    if (errors.length > 0) {
        errorMessage.innerHTML = errors.join("<br>");
        errorMessage.style.display = "block";
        return;
    }




    try {

        //送信データ整形
        const addUserData = {
            userName: userName,
            password: password
        };

        //POST送信
        const response = await fetch("http://172.16.7.24:5265/api/users", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(addUserData)
        });

        //レスポンス処理
        //成功したらログインユーザー確定して、チャット画面へ
        if (response.status === 201) {
            console.info("新規登録成功");
            alert("ユーザー登録が完了しました");
            window.location.href = "Login.html";
        }
        else if (response.status === 400) {
            alert("入力値が不正です");
        }
        else if (response.status === 409) {
            errors.push("ユーザー名が重複しています。<br>別の名前にしてください。");
            errorMessage.innerHTML = errors.join("<br>");
            errorMessage.style.display = "block";
        }
        else if (response.status === 500) {
            alert("サーバーエラー");
        }

    }
    catch (error) {
        console.error(error);
        alert("通信に失敗しました");
    }

});