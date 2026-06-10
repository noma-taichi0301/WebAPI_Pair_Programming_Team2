//ログインボタン押下処理
document.getElementById("loginButton").addEventListener("click", async () => {

    console.info("ログインボタン押下");

    //入力取得
    const userName = document.getElementById("userName").value;
    const password = document.getElementById("password").value;

    //入力チェック
    if (userName === "") {
        alert("ユーザー名を入力してください");
        return;
    }

    if (password === "") {
        alert("パスワードを入力してください");
        return;
    }

    
    try {

        //送信データ整形
        const loginData = {
            userName: userName,
            password: password
        };

        //POST送信
        const response = await fetch("/api/login", {
            method: "POST",
            headers:{"Content-Type": "application/json"},
            body: JSON.stringify(loginData)
        });

        //レスポンス処理
        //成功したらログインユーザー確定して、チャット画面へ
        if (response.status === 200) {

            console.info("ログイン成功");
            //レスポンスをJSONに変換
            const loginUser = await response.json();

            //ブラウザにデータを保存してチャット画面に遷移
            localStorage.setItem("userId", loginUser.userId);
            localStorage.setItem("userName", loginUser.userName);
            window.location.href = "Chat.html";
        }
        else if (response.status === 400){
            alert("入力値が不正です");
        }
        else if (response.status === 401) {
            alert("ユーザー名またはパスワードが正しくありません");
        }
        else if (response.status === 500){
            alert("サーバーエラー");
        }

    }
    catch (error) {
        console.error("ログイン通信失敗", error);
        alert("通信に失敗しました");
    }

});