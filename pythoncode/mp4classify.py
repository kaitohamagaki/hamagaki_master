import numpy as np
from PIL import Image
from keras.models import load_model
import cv2
import time
import pandas as pd

imsize = (64, 64)

def moving_average(array, window_size):
    cumsum_vec = np.cumsum(np.insert(array, 0, 0)) 
    ma_vec = (cumsum_vec[window_size:] - cumsum_vec[:-window_size]) / window_size
    return np.concatenate((np.zeros(window_size-1), ma_vec))

def load_image(path):
    img = Image.open(path)
    img = img.convert('RGB')
    img = img.resize(imsize)
    img = np.asarray(img)
    img = img / 255.0
    return img

# 更新された絶対パス
keras_param = "F:\MTG\\newcode\\walk_jump.h5"
arr = []
timearr = []
model = load_model(keras_param)

def save_frame_camera_cycle(video_file_path, cycle, delay=1, window_name='frame'):
    X_center = []
    Y_center = []
    window = 15 # 移動平均の範囲

    y_line_min = 0
    y_line_max = 0
    yline = 0

    speed = []
    speeddata1 = 0
    speeddata2 = 0
    
    cap = cv2.VideoCapture(video_file_path)

    if not cap.isOpened():
        return
    j = 0
    n = 0

    while True:
        ret, frame = cap.read()
        
        if n == cycle:
            n = 0
            start = time.time()
            # 更新された絶対パス
            cv2.imwrite('F:\MTG\\newcode\\cameracap\\pic_'+str(j).zfill(4)+'.jpg', frame)
            testpic = 'F:\MTG\\newcode\\cameracap\\pic_'+str(j).zfill(4)+'.jpg'
            
            img = load_image(testpic)
            prd = model.predict(np.array([img]))
            prelabel = np.argmax(prd, axis=1)
            if prelabel == 0:
                # print(str(j) + "フレーム目　>>> walk")
                print("walk")
                arr.append(0)
            elif prelabel == 1:
                # print(str(j) + "フレーム目　>>> jump")
                print("jump")
                arr.append(1)

            hsv = cv2.cvtColor(frame, cv2.COLOR_BGR2HSV)
                # 白色のHSVの値域（広範囲に設定）
            hsv_min = np.array([5, 37, 186])  # 最小のVを使用
            hsv_max = np.array([45, 77, 243])  # 最大のVを使用
            mask = cv2.inRange(hsv, hsv_min, hsv_max)
        
            contours, _ = cv2.findContours(mask, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)
            t = 0
            for contour in contours:
                moments = cv2.moments(contour)
                if moments["m00"] == 0:
                    continue
                if moments["m00"] > 1000:
                    cx = int(moments["m10"] / moments["m00"])
                    cy = int(moments["m01"] / moments["m00"])
                else:
                    continue
                frame = cv2.circle(frame, (cx, cy), 5, (0, 0, 255), -1)
                if t == 0:
                    X_center.append(cx)
                    Y_center.append(cy)
                    t += 1
                    if y_line_min == 0:
                        y_line_min = cy
                        y_line_max = cy
                    if cy < y_line_min:
                        cy = y_line_min
                    if cy > y_line_max:
                        cy = y_line_max  
                    yline = y_line_min + (y_line_max - y_line_min)/2
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
            cv2.imshow("Result", frame)
            if cv2.waitKey(1) & 0xFF == ord("q"):
                break
        end = time.time()

        if j > 500:
            ##print(speed)
            # 更新された絶対パス
            ##pd.DataFrame(Y_center).to_csv('F:\\MTG\\20230911\\y_center_2.csv', index=False, header=False)
            break
        n += 1
        j += 1

# 更新された絶対パス
save_frame_camera_cycle('F:\\MTG\\20230911\\walk_and_jump.mp4',1)
