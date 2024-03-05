import numpy as np
from PIL import Image
from keras.models import load_model
import cv2
import time
import pandas as pd

imsize = (64, 64)

# モデルをロード
keras_param = "F:\MTG\\newcode\\walk_jump.h5"
model = load_model(keras_param)

def load_image_in_memory(image):
    img = image.convert('RGB')
    img = img.resize(imsize)
    img = np.asarray(img)
    img = img / 255.0
    return img

def save_frame_camera_cycle(cycle, delay=1, window_name='frame'):
    X_center = []
    Y_center = []
    window = 15 # 移動平均の範囲
    y_line_min = 0
    y_line_max = 0
    yline = 0
    speed = []
    speeddata1 = 0
    speeddata2 = 0
    
    # ここで0を指定してWebカメラからの映像を読み込む
    cap = cv2.VideoCapture(0)

    if not cap.isOpened():
        return

    j = 0
    n = 0
    while True:
        ret, frame = cap.read()
        if not ret:
            break
        
        if n == cycle:
            n = 0
            start = time.time()
            
            # リアルタイムの画像処理
            img = Image.fromarray(cv2.cvtColor(frame, cv2.COLOR_BGR2RGB))
            img = load_image_in_memory(img)
            prd = model.predict(np.array([img]))
            prelabel = np.argmax(prd, axis=1)
            if prelabel == 0:
                print("walk")
            elif prelabel == 1:
                print("jump")

            # HSV変換とマスクの適用
            hsv = cv2.cvtColor(frame, cv2.COLOR_BGR2HSV)
            hsv_min = np.array([5, 37, 186])
            hsv_max = np.array([45, 77, 243])
            mask = cv2.inRange(hsv, hsv_min, hsv_max)
            
            # 輪郭の検出
            contours, _ = cv2.findContours(mask, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)
            for contour in contours:
                moments = cv2.moments(contour)
                if moments["m00"] < 1000:
                    continue
                cx = int(moments["m10"] / moments["m00"])
                cy = int(moments["m01"] / moments["m00"])
                frame = cv2.circle(frame, (cx, cy), 5, (0, 0, 255), -1)
                X_center.append(cx)
                Y_center.append(cy)
                if y_line_min == 0 or cy < y_line_min:
                    y_line_min = cy
                if cy > y_line_max:
                    y_line_max = cy
                yline = y_line_min + (y_line_max - y_line_min) / 2
                
                # 速度計算
                if len(Y_center) > 3:
                    if Y_center[-2] < yline and yline < Y_center[-1]:
                        speeddata1 = len(X_center)
                    if Y_center[-2] > yline and yline > Y_center[-1]:
                        speeddata2 = len(Y_center)
                    if speeddata1 !=0 and speeddata2 !=0:
                        speedtmp = abs(speeddata2-speeddata1)
                        speed.append(speedtmp)
                        ##print('speed = ',speedtmp)
                        speeddata1 = 0
                        speeddata2 = 0   
                

            cv2.imshow(window_name, frame)
            if cv2.waitKey(delay) & 0xFF == ord('q'):
                break

            end = time.time()

        if j > 5000:
            break

        n += 1
        j += 1

    cap.release()
    cv2.destroyAllWindows()

# Webカメラからの映像をリアルタイムで処理する関数を呼び出す
save_frame_camera_cycle(cycle=1)
