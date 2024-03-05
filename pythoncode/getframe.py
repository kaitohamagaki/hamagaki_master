import cv2
import os

# 入力動画のパスを指定する
video_name = 'walk'
video_path = '{}.mp4'.format(video_name)

# 保存先フォルダが存在しない場合は作成する
if not os.path.exists(video_name):
    os.makedirs(video_name)

# カメラからのキャプチャを開始
cap = cv2.VideoCapture(video_path)

# 画像の枚数をカウントする変数
count = 0

while True:
    # 1フレーム読み込む
    ret, frame = cap.read()
    if not ret:
        break

    # 画像を保存する
    cv2.imwrite('{}/{}_{}.jpg'.format(video_name, video_name, count), frame)

    # 画像の枚数をカウントする
    count += 1

# 動画を閉じる
cap.release()
cv2.destroyAllWindows()
