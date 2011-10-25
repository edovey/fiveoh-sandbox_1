//
//  DiseaseView.h
//  BDViewer
//
//  Created by Liz Dovey on 11-10-19.
//  Copyright (c) 2011 875953 Alberta, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface DiseaseView : UIViewController <UITableViewDelegate, UITableViewDataSource, UIWebViewDelegate>
{
    NSArray * diseaseArray;
    NSString *parentId;
    UITableView * dataTableView;
    NSString *parentName;
}

@property (nonatomic, retain) NSArray * diseaseArray;
@property (retain, nonatomic) NSString *parentId;
@property (retain, nonatomic) IBOutlet UITableView *dataTableView;
@property (nonatomic, retain) NSString *parentName;

-(id)initWithParentId:(NSString *)pParentId withParentName:(NSString *)pParentName;
@end
