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
    NSString *parentId;
    NSString *diseaseId;
    NSString *overviewHTMLString;
    NSString *detailHTMLString;
}

@property (retain, nonatomic) IBOutlet UIWebView *dataWebView;
@property (retain, nonatomic) NSString *parentId;
@property (retain, nonatomic) NSString *diseaseId;
@property (retain, nonatomic) NSString *overviewHTMLString;
@property (retain, nonatomic) NSString *detailHTMLString;

@end
