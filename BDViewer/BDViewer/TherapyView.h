//
//  TherapyView.h
//  BDViewer
//
//  Created by Liz Dovey on 11-10-20.
//  Copyright (c) 2011 875953 Alberta, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface TherapyView : UIViewController <UIWebViewDelegate>
{
    UIWebView *dataWebView;
    NSArray *therapyArray;
    NSString *parentId;
    NSString *diseaseId;
    NSString *overviewHTMLString;
}

@property (retain, nonatomic) IBOutlet UIWebView *dataWebView;
@property (retain, nonatomic) NSArray *therapyArray;
@property (retain, nonatomic) NSString *parentId;
@property (retain, nonatomic) NSString *diseaseId;
@property (retain, nonatomic) NSString *overviewHTMLString;

@end
