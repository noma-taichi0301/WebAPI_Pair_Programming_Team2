//ログインボタン押下処理
document.getElementById("loginButton").addEventListener("click", async () => {

    console.info("ログインボタン押下");

    //入力取得
    const userName = document.getElementById("userName").value;
    const password = document.getElementById("password").value;

    //入力チェック
    //エラーメッセージを削除
    const errorMessage = document.getElementById("errorMessage");
    errorMessage.textContent = "";
    errorMessage.style.display = "none";

    let errors = [];

    if (userName === "") {
        errors.push("ユーザー名を入力してください");
    }

    if (password === "") {
        errors.push("パスワードを入力してください");
    }

    if (errors.length > 0) {
        errorMessage.innerHTML = errors.join("<br>");
        errorMessage.style.display = "block";
        console.info("ログイン不可");
        return;
    }

    
    try {

        //送信データ整形
        const loginData = {
            userName: userName,
            password: password
        };

        //ログインPOST送信
        const response = await fetch("http://172.16.7.24:5265/api/login", {
            method: "POST",
            headers:{"Content-Type": "application/json"},
            body: JSON.stringify(loginData)
        });

        //レスポンス処理
        //成功したらログインユーザー確定して、チャット画面へ
        if (response.status === 200) {

            console.info("ログイン成功");

            //ブラウザにデータを保存してチャット画面に遷移
            localStorage.setItem("userName", userName);
            window.location.href = "Chat.html";

        }
        else if (response.status === 400) {
            console.warn("入力値不正");
            alert("入力値が不正です");

        }
        else if (response.status === 401) {
            console.warn("ログイン失敗");
            errors.push("ユーザー名またはパスワードが正しくありません");
            errorMessage.innerHTML = errors.join("<br>");
            errorMessage.style.display = "block";

        }
        else if (response.status === 500){
            console.warn("サーバーエラー");
            alert("サーバーエラー");

        }

    }
    catch (error) {
        alert("通信に失敗しました");
        console.error("ログイン通信失敗", error);

    }

});