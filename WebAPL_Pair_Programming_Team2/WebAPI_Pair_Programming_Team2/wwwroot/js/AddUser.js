//新規登録ボタン押下処理
document.getElementById("addUserButton").addEventListener("click", async () => {

    console.info("新規登録ボタン押下");

    //入力取得
    const userName = document.getElementById("addUserName").value;
    const password = document.getElementById("addPassword").value;

    //入力チェック
    if (userName === "") {
        alert("ユーザー名を入力してください");
        return;
    }

    if (userName.length > 30 ) {
        alert("ユーザー名は30文字以下にしてください");
        return;
    }

    if (password === "") {
        alert("パスワードを入力してください");
        return;
    }

    if (password.length > 30 || password.length < 8) {
        alert("パスワードは8文字以上、30文字以下にしてください");
        return;
    }


    try {

        //送信データ整形
        const AddUserData = {
            userName: userName,
            password: password
        };

        //POST送信
        const response = await fetch("/api/users", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(AddUserData)
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
        else if (response.status === 500) {
            alert("サーバーエラー");
        }

    }
    catch (error) {
        console.error(error);
        alert("通信に失敗しました");
    }

});