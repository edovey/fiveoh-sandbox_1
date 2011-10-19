//
//  ChapterView.h
//  BDViewer
//
//  Created by Liz Dovey on 11-10-19.
//  Copyright (c) 2011 875953 Alberta, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface ChapterView : UIViewController <UITableViewDelegate, UITableViewDataSource>
{
    UITableView *dataTableView;
    NSArray *chapterArray;
}
@property (retain, nonatomic) IBOutlet UITableView *dataTableView;

@property (retain, nonatomic) NSArray *chapterArray;
@end
