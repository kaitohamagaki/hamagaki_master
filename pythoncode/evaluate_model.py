import numpy as np
from keras.models import load_model
from keras.utils import np_utils

# クラスと画像サイズの設定
classes = ["walk", "jump"]
num_classes = len(classes)
image_size = 64

def load_data():
    """
    テストデータを読み込む関数
    """
    _, X_test, _, y_test = np.load("./walk_jump.npy", allow_pickle=True)
    X_test = X_test.astype("float") / 255
    y_test = np_utils.to_categorical(y_test, num_classes)
    return X_test, y_test

def main():
    # テストデータの読み込み
    X_test, y_test = load_data()

    # モデルの読み込み
    model = load_model('./walk_jump.h5')

    # モデルの評価
    loss, accuracy = model.evaluate(X_test, y_test)
    print("Test Loss:", loss)
    print("Test Accuracy:", accuracy)

if __name__ == "__main__":
    main()
