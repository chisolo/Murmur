#import <Foundation/Foundation.h>
#import <FlowHwSDK/FlowHwSDK.h>

// 定义sdk回调类型
typedef void(*Flow998_iosCallback)(long idx, int ret, const char* msg, const char* data);

typedef void(^Flow998_chargeFn)(FHSDKInt * _Nonnull ret, NSString * _Nonnull msg, FHSDKFlow998_DataCharge * _Nullable data);

NSString* bytesToString(const char* bytes) {
    return [NSString stringWithUTF8String:bytes != NULL ? bytes : ""];
}

/*
 如果直接[NSString UTF8String]，那么对于oc则该变量没有其他引用，则会被gc所释放内存，
 从而导致c#那边读取时可能出现指针异常。
 */
const char* stringToBytes(NSString* s) {
    if (s == NULL) {
        return NULL;
    }
    const char* input = [s UTF8String];
    NSUInteger inputLen = [s lengthOfBytesUsingEncoding:NSUTF8StringEncoding];
    char* output = (char*)malloc(inputLen + 1);
    memcpy(output, input, inputLen);
    output[inputLen] = 0;
    return output;
}

void Flow998_init(long idx, Flow998_iosCallback callback, const char* options) {
    NSString* s = bytesToString(options);
    FHSDKFlow998_InitOptions* options1 = [[FHSDKFlow998_InitOptionsCompanion shared] parseS:s];
    [[FHSDKFlow998_SDK shared] doInitOptions:options1
                                          fn:^(FHSDKInt * _Nonnull ret, NSString * _Nonnull msg, FHSDKFlow998_DataInit * _Nullable data) {
        callback(idx, ret.intValue, stringToBytes(msg), stringToBytes(data != NULL ? [data toString] : NULL));
    }];
}

void Flow998_abtestConfig(long idx, Flow998_iosCallback callback) {
    [[FHSDKFlow998_SDK shared] abtestConfigFn:^(FHSDKInt * _Nonnull ret, NSString * _Nonnull msg, NSDictionary<NSString *,NSString *> * _Nullable data) {
        NSString* data1 = data != NULL ? [[FHSDKFlow998_SDK shared] jsonEncodeObjectData:data] : NULL;
        callback(idx, ret.intValue, stringToBytes(msg), stringToBytes(data1));
    }];
}

void Flow998_pushEvent(const char*  _event, const char*  eventData) {
    NSString* s = bytesToString(eventData);
    NSDictionary<NSString*, NSString*>* dict = [[FHSDKFlow998_SDK shared] jsonDecodeObjectData: s];
    [[FHSDKFlow998_SDK shared] pushEventEvent:[[NSString alloc] initWithUTF8String:_event]
                                         data:dict];
}

const char *Flow998_getPrivacyURL(void) {
    NSString* s = [[FHSDKFlow998_SDK shared] getPrivacyURL];
    return stringToBytes(s);
}

const char *Flow998_getUserURL(void) {
    NSString* s = [[FHSDKFlow998_SDK shared] getUserURL];
    return stringToBytes(s);
}

void Flow998_login(long loginIdx, Flow998_iosCallback loginCallback, Flow998_iosCallback chargeCallback) {
    Flow998_chargeFn chargeFn = NULL;
    if (chargeCallback != NULL) {
        chargeFn = ^(FHSDKInt * _Nonnull ret, NSString * _Nonnull msg, FHSDKFlow998_DataCharge * _Nullable data) {
            chargeCallback(loginIdx, ret.intValue, stringToBytes(msg), stringToBytes([data toString]));
        };
    }
    [[FHSDKFlow998_SDK shared] loginFn:^(FHSDKInt * _Nonnull ret, NSString * _Nonnull msg, FHSDKFlow998_DataLogin * _Nullable data) {
        loginCallback(loginIdx, ret.intValue, stringToBytes(msg), stringToBytes([data toString]));
    }
                              chargeFn:chargeFn];
}

void Flow998_customLoginToken(long loginIdx, Flow998_iosCallback loginCallback, Flow998_iosCallback chargeCallback) {
    Flow998_chargeFn chargeFn = NULL;
    if (chargeCallback != NULL) {
        chargeFn = ^(FHSDKInt * _Nonnull ret, NSString * _Nonnull msg, FHSDKFlow998_DataCharge * _Nullable data) {
            chargeCallback(loginIdx, ret.intValue, stringToBytes(msg), stringToBytes([data toString]));
        };
    }
    [[FHSDKFlow998_SDK shared] customLoginTokenFn:^(FHSDKInt * _Nonnull ret, NSString * _Nonnull msg, FHSDKFlow998_DataLogin * _Nullable data) {
        loginCallback(loginIdx, ret.intValue, stringToBytes(msg), stringToBytes([data toString]));
    } chargeFn:chargeFn];
}

void Flow998_customLogin(long loginIdx, const char* type, Flow998_iosCallback loginCallback, Flow998_iosCallback chargeCallback) {
    NSString* s = bytesToString(type);
    Flow998_chargeFn chargeFn = NULL;
    if (chargeCallback != NULL) {
        chargeFn = ^(FHSDKInt * _Nonnull ret, NSString * _Nonnull msg, FHSDKFlow998_DataCharge * _Nullable data) {
            chargeCallback(loginIdx, ret.intValue, stringToBytes(msg), stringToBytes([data toString]));
        };
    }
    [[FHSDKFlow998_SDK shared] customLoginType:(s) fn:^(FHSDKInt * _Nonnull ret, NSString * _Nonnull msg, FHSDKFlow998_DataLogin * _Nullable data) {
        loginCallback(loginIdx, ret.intValue, stringToBytes(msg), stringToBytes([data toString]));
    } chargeFn:chargeFn];
}

void Flow998_logout(long idx, Flow998_iosCallback callback) {
    [[FHSDKFlow998_SDK shared] logoutFn:^(FHSDKInt * _Nonnull ret, NSString * _Nonnull msg, id _Nullable data) {
        callback(idx, ret.intValue, stringToBytes(msg), NULL);
    }];
}

void Flow998_link(long idx, const char* type, bool shouldSwitch, Flow998_iosCallback loginCallback){
    NSString* s = bytesToString(type);
    [[FHSDKFlow998_SDK shared] linkType:(s)shouldSwitch:(shouldSwitch) fn:^(FHSDKInt * _Nonnull ret, NSString * _Nonnull msg, FHSDKFlow998_DataLogin * _Nullable data) {
        loginCallback(idx, ret.intValue, stringToBytes(msg), stringToBytes([data toString]));
    }];
}

const char* Flow998_getLinklist(void) {
    NSArray<NSString *> * linkedAry = [[FHSDKFlow998_SDK shared] getLinkedList];
    NSString* result = [linkedAry componentsJoinedByString:@","];
    return stringToBytes(result);
}

void Flow998_updateUser(long idx, const char *name, const char *photo, Flow998_iosCallback callback) {
    NSString* name1 = bytesToString(name);
    NSString* photo1 = bytesToString(photo);
    FHSDKFlow998_UpdateUserOptions* options = [[FHSDKFlow998_UpdateUserOptions alloc] initWithName:name1
                                                                                             photo:photo1];
    [[FHSDKFlow998_SDK shared] updateUserOptions:options fn:^(FHSDKInt * _Nonnull ret, NSString * _Nonnull msg, id _Nullable data) {
        callback(idx, ret.intValue, stringToBytes(msg), NULL);
    }];
}

const char* Flow998_getUser(void) {
    FHSDKFlow998_DataGetUser* data = [[FHSDKFlow998_SDK shared] getUser];
    return stringToBytes([data toString]);
}

void Flow998_appAdOpenAdLoad(void) {
    [[FHSDKFlow998_SDK shared] appOpenAdLoad];
}

void Flow998_appOpenAdStart(long idx, const char* pname, Flow998_iosCallback callback) {
    NSString* s = bytesToString(pname);
    [[FHSDKFlow998_SDK shared] appOpenAdStartPname:(s) fn:^(FHSDKInt * _Nonnull ret, NSString * _Nonnull msg, NSString * _Nullable data) {
        callback(idx, ret.intValue, stringToBytes(msg), stringToBytes(data));
    }];
}

void Flow998_appOpenAdStop(void) {
    [[FHSDKFlow998_SDK shared] appOpenAdStop];
}

void Flow998_rewardAdPreload(long idx, Flow998_iosCallback callback, const char* unionId) {
    NSString* s = bytesToString(unionId);
    [[FHSDKFlow998_SDK shared] rewardAdPreloadUnionId:s
                                                   fn:^(FHSDKInt * _Nonnull ret, NSString * _Nonnull msg, NSString * _Nullable data) {
        callback(idx, ret.intValue, stringToBytes(msg), stringToBytes(data));
    }];
}

void Flow998_rewardShow(long idx, Flow998_iosCallback callback, const char* unionId, const char* pname) {
    NSString* s1 = bytesToString(unionId);
    NSString* s2 = bytesToString(pname);
    [[FHSDKFlow998_SDK shared] rewardAdShowUnionId:s1
                                             pname:s2
                                                fn:^(FHSDKInt * _Nonnull ret, NSString * _Nonnull msg, NSString * _Nullable data) {
        callback(idx, ret.intValue, stringToBytes(msg), stringToBytes(data));
    }];
}

void Flow998_fullVideoAdPreload(const char* unionId){
    NSString* s = bytesToString(unionId);
    [[FHSDKFlow998_SDK shared] fullVideoAdPreloadUnionId:s];
}

void Flow998_fullVideoAdShow(long idx, Flow998_iosCallback callback, const char* unionId, const char* pname) {
    NSString* s1 = bytesToString(unionId);
    NSString* s2 = bytesToString(pname);
    [[FHSDKFlow998_SDK shared] fullVideoAdShowUnionId:s1
                                                pname:s2
                                                   fn:^(FHSDKInt * _Nonnull ret, NSString * _Nonnull msg, NSString * _Nullable data) {
        callback(idx, ret.intValue, stringToBytes(msg), stringToBytes(data));
    }];
}

int Flow998_bannerAdFitHeightPx(void) {
    return [[FHSDKFlow998_SDK shared] bannerAdFitHeightPx];
}

void Flow998_bannerAdInit(const char* data) {
    NSString* s = bytesToString(data);
    NSArray<FHSDKFlow998_BannerOptions*>* list = [[FHSDKFlow998_BannerOptionsCompanion shared] parseJsonArray:s];
    if (list != NULL) {
        [[FHSDKFlow998_SDK shared] bannerAdInitList:list];
    }
}

void Flow998_bannerAdShow(const char* data) {
    NSString* s = bytesToString(data);
    NSArray<NSString*>* arrayData = [[FHSDKFlow998_SDK shared] jsonDecodeArrayData:s];
    [[FHSDKFlow998_SDK shared] bannerAdShowPnames:arrayData];
}

void Flow998_chargeSetCallback(long idx, Flow998_iosCallback chargeCallback) {
    [[FHSDKFlow998_SDK shared] chargeSetCallbackFn:^(FHSDKInt * _Nonnull ret, NSString * _Nonnull msg, FHSDKFlow998_DataCharge * _Nullable data) {
        chargeCallback(idx, ret.intValue, stringToBytes(msg), stringToBytes([data toString]));
    }];
}

void Flow998_charge(const char* productId, const char* consumeExt) {
    FHSDKFlow998_ChargeOptions* option = [[FHSDKFlow998_ChargeOptions alloc] initWithProductId:bytesToString(productId)
                                                                                           ext:bytesToString(consumeExt)];
    [[FHSDKFlow998_SDK shared] chargeOptions:option];
}

void Flow998_chargeRecovery(void) {
    [[FHSDKFlow998_SDK shared] chargeRecovery];
}

void Flow998_chargeProducts(long idx, Flow998_iosCallback callback, const char* productIds) {
    NSString* s = bytesToString(productIds);
    NSArray<NSString*>* productArray = [[FHSDKFlow998_SDK shared] jsonDecodeArrayData:s];
    [[FHSDKFlow998_SDK shared] chargeProductsList:productArray
                                               fn:^(FHSDKInt * _Nonnull ret, NSString * _Nonnull msg, NSArray<FHSDKFlow998_DataChargeProduct *> * _Nullable data) {
        NSString* data1 = NULL;
        if (data != NULL) {
            data1 = [[FHSDKFlow998_DataChargeProductCompanion shared] encodeList:data];
        }
        callback(idx, ret.intValue, stringToBytes(msg), stringToBytes(data1));
    }];
}

void Flow998_chanrgeFinish(const char* orderSn) {
    [[FHSDKFlow998_SDK shared] chargeFinishOrderSn:bytesToString(orderSn)];
}

void Flow998_requestReview(long idx, Flow998_iosCallback callback) {
    [[FHSDKFlow998_SDK shared] requestReviewFn:^(FHSDKInt * _Nonnull ret, NSString * _Nonnull msg, id _Nullable data) {
        callback(idx, ret.intValue, stringToBytes(msg), NULL);
    }];
}

void Flow998_socialInvite(long idx, Flow998_iosCallback callback, const char* type, const char* name, const char* ext) {
    [[FHSDKFlow998_SDK shared] socialInviteType:bytesToString(type) fn:^(FHSDKInt * _Nonnull ret, NSString * _Nonnull msg, id _Nullable data) {
        callback(idx, ret.intValue, stringToBytes(msg), NULL);
    }];
}

void Flow998_socialInviteCount(long idx, const char* name, Flow998_iosCallback callback) {
    [[FHSDKFlow998_SDK shared] socialInviteCountName:bytesToString(name) fn:^(FHSDKInt * _Nonnull ret, NSString * _Nonnull msg, FHSDKInt * _Nullable data) {
        NSString* data1 = data != NULL ? data.stringValue : NULL;
        callback(idx, ret.intValue, stringToBytes(msg), stringToBytes(data1));
    }];
}

void Flow998_socialFrom(long idx, Flow998_iosCallback callback) {
    [[FHSDKFlow998_SDK shared] socialFromFn:^(FHSDKInt * _Nonnull ret, NSString * _Nonnull msg, FHSDKFlow998_DataSocialFrom * _Nullable data) {
        callback(idx, ret.intValue, stringToBytes(msg), data != NULL ? stringToBytes([data toString]) : NULL);
    }];
}

void Flow998_socialInviteReward(void) {
    [[FHSDKFlow998_SDK shared] socialInviteReward];
}

void Flow998_lbScore(int score) {
    [[FHSDKFlow998_SDK shared] lbScoreScore:score];
}

void Flow998_lbQueryInit(const char* options) {
    FHSDKFlow998_LbQueryInitOptions* options1 = [[FHSDKFlow998_LbQueryInitOptions companion] parseS:bytesToString(options)];
    if (options1 == NULL) {
        return;
    }
    [[FHSDKFlow998_SDK shared] lbQueryInitOptions:options1];
}

void Flow998_lbGlobalQuery(long idx, bool up, Flow998_iosCallback callback) {
    [[FHSDKFlow998_SDK shared] lbGlobalQueryUp:up fn:^(FHSDKInt * _Nonnull ret, NSString * _Nonnull msg, NSArray<FHSDKFlow998_DataLbQuery *> * _Nullable data) {
        NSString* data1 = NULL;
        if (data != NULL) {
            data1 = [[FHSDKFlow998_DataLbQuery companion] encodeList:data];
        }
        callback(idx, ret.intValue, stringToBytes(msg), stringToBytes(data1));
    }];
}
